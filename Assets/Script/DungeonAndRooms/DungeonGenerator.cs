using System;
using System.Collections.Generic;
using UnityEngine;

// Singleton class Responsible for generating the dungeon
public class DungeonGenerator : MonoBehaviour
{
    private static DungeonGenerator Instance;

    [SerializeField]
    private float roomWidth = 60.0f; // The width of a Room size unit
    [SerializeField]
    private float roomHeight = 40.0f; // The width of a Room size unit

    // Width and height are in Room size (see Room.cs file for detailed explanation)
    [SerializeField]
    private int playAreaWidth = 15;
    [SerializeField]
    private int playAreaHeight = 15;
    
    // Offset from the borders of the play area delimiting the area in which the first room can be fined
    [SerializeField]
    private int firstRoomBoundary = 3; 

    [SerializeField]
    private GameObject[] normalRoomPrefabs; // Set of non-boss rooms to pick from to generate the dungeon
    [SerializeField]
    private GameObject[] bossRoomsPrefabs; // Set of boss rooms to pick from

    [SerializeField]
    private int minRoomsBetweenStartAndBoss = 3;
    [SerializeField]
    private int maxRoomsBetweenStartAndBoss = 7;

    [SerializeField]
    private int minNumSideRooms = 3;
    [SerializeField]
    private int maxNumSideRooms = 5;

    [SerializeField]
    private Transform roomTransform;

    [SerializeField]
    private RectTransform playerTransform;

    private List<Room> roomsInDungeon = new List<Room>();
    private int encounterIndex = 0;

    private void Awake()
    {
        AudioManagerScript.instance.Play("DoorSound2");
        Instance = this;
    }

    public static void GenerateDungeon(string seed)
    {
        GenerateDungeon(seed.GetHashCode());
    }

    public static void GenerateDungeon(int seed)
    {
        UnityEngine.Random.InitState(seed);
        DungeonManager.SetSeed(seed);
        Instance.GenerateDungeonImpl();
    }

    public static void GenerateDungeon()
    {
        // Get a random seed and pass it to the GenerateDungon(int) function
        int seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        GenerateDungeon(seed);
    }

    // Loads the dungeon from the provided data
    public static void LoadDungeon(int seed, List<EncounterData> encounters)
    {
        UnityEngine.Random.InitState(seed);
        Instance.GenerateDungeonImpl(encounters);
    }

    // Underlying implementation of the GenerateDungeon function
    private void GenerateDungeonImpl(List<EncounterData> encounters = null)
    {
        ClearPreviousDungeon();
        playerTransform.anchoredPosition = Vector3.zero;
        Room[,] dungeon = new Room[playAreaWidth, playAreaHeight];


        if (encounters != null)
        {
            encounterIndex = 0;
        }

        Vector2Int currPosition = new Vector2Int(UnityEngine.Random.Range(firstRoomBoundary, 
            playAreaWidth - firstRoomBoundary), UnityEngine.Random.Range(firstRoomBoundary, 
            playAreaHeight-firstRoomBoundary));

        // Leave the first room without any enemies.
        PickAndCreateRoom(dungeon, currPosition, normalRoomPrefabs, null, false); 

        int numRoomsBetweenBossStart = UnityEngine.Random.Range(minRoomsBetweenStartAndBoss,
            maxRoomsBetweenStartAndBoss);

        // First pass, tries to create a singular path from the starting room to the boss room
        for (int i = 0; i < numRoomsBetweenBossStart; i++)
        {
            Vector2Int newPos = GenerateNewRoom(dungeon, currPosition, normalRoomPrefabs, GetNextEncounter(encounters));

            if (newPos == new Vector2Int(-1, -1))
            {
                //if we reached a dead end find a new position to start from
                newPos = FindExistingRoom(dungeon, currPosition);
                i--; // Decrement i so we don't skip a room
            }

            currPosition = newPos; // Update current position
        }

        // Generate Boss Room
        GenerateNewRoom(dungeon, currPosition, bossRoomsPrefabs, GetNextEncounter(encounters));

        // Second pass, add random side rooms to the existing dungeon
        int numSideRooms = UnityEngine.Random.Range(minNumSideRooms, maxNumSideRooms);

        for (int i = 0; i < numSideRooms; i++)
        {
            currPosition = FindExistingRoom(dungeon, currPosition);
            Vector2Int newPos = GenerateNewRoom(dungeon, currPosition, normalRoomPrefabs, GetNextEncounter(encounters));

            if (newPos == new Vector2Int(-1, -1))
            {
                i--; //if room not properly generated decrement i to not skip a room and try again
                continue;
            }
            currPosition = newPos;
        }
    }

    // Generates new room next to the room at the current position and returns the new 
    // coordinate position of the last rooms placed
    // returns Vector2Int(-1, -1) if no new room could be placed from the provided coordinate
    private Vector2Int GenerateNewRoom(Room[,] dungeon, Vector2Int currPos, GameObject[] roomPrefabs, EncounterData e)
    {
        Vector2Int nextPos = GetNextPos(dungeon, currPos);

        if (nextPos == new Vector2Int(-1, -1))
        {
            return nextPos;
        }

        PickAndCreateRoom(dungeon, nextPos, roomPrefabs, e);
        return nextPos;
    }

    private void PickAndCreateRoom(Room[,] dungeon, Vector2Int currPos, GameObject[] roomPrefabs, 
        EncounterData e = null, bool generateEncounter = true)
    {
        // Assumes for now that all rooms have size 1x1 (will need to be modified later on)
        GameObject currPrefab = roomPrefabs[UnityEngine.Random.Range(0, roomPrefabs.Length)];

        while (!RoomCanFit(dungeon, currPrefab.GetComponent<Room>(), currPos))
        {
            currPrefab = roomPrefabs[UnityEngine.Random.Range(0, roomPrefabs.Length)];
        }

        Room r = InstantiateRoom(dungeon, currPrefab, currPos, generateEncounter && e == null);
        if (generateEncounter && e != null)
        {
            r.SetEncounter(e);
        }
    }

    private Room InstantiateRoom(Room[,] dungeon, GameObject prefabToInstantiate, Vector2Int coord, 
        bool generateEncounter = false)
    {
        /* Commented out but kept in case we want to have rooms not on top of one another
         * 
        GameObject newRoomGO = Instantiate(prefabToInstantiate.gameObject, new Vector3(coord.x * roomWidth, 
            coord.y * roomHeight, 0), Quaternion.identity, gameplayTransform);
         */

        GameObject newRoomGO = Instantiate(prefabToInstantiate, Vector3.zero, 
            Quaternion.identity, roomTransform);

        newRoomGO.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

        if (roomsInDungeon.Count == 0)
        {
            newRoomGO.SetActive(true); // Activate first room
            newRoomGO.GetComponent<Room>().UnlockRoom();
        }
        else
        {
            // Deactivate every other room (prefab needs to have gameobject activated to work properly 
            // so we need to disable the rooms through code after)
            newRoomGO.SetActive(false); 
        }


        Room newRoom = newRoomGO.GetComponent<Room>();
        newRoom.SetBottomLeftCornerWorldCoord(coord);
        SetRoomInDungeon(dungeon, newRoom, coord);
        roomsInDungeon.Add(newRoom);

        if (generateEncounter)
        {
            newRoom.SetRandomEncounter();
        }

        return newRoom;
    }

    private Vector2Int GetNextPos(Room[,] dungeon, Vector2Int currPos)
    {
        int enumLength = Enum.GetNames(typeof(Enums.Direction)).Length;
        Enums.Direction nextDirection = (Enums.Direction)UnityEngine.Random.Range(1, enumLength);

        Vector2Int nextPos = DirectionToCoord(nextDirection, currPos);

        for (int i = 1; !IsCoordValid(dungeon, nextPos) && i < enumLength; i++)
        {
            nextDirection = (Enums.Direction)(((int)nextDirection + 1) % enumLength);

            //skip the Enums.Direction.NA 
            if (nextDirection == 0)
            {
                nextDirection = (Enums.Direction)1;
            }

            nextPos = DirectionToCoord(nextDirection, currPos);
        }

        if (!IsCoordValid(dungeon, nextPos))
        {
            return new Vector2Int(-1, -1);
        }
        return nextPos;
    }

    // Inserts the provided room into the dungeon and links all of it's doors to the nearby rooms. 
    // The coordinate provided is the bottom left corner of the room
    private void SetRoomInDungeon(Room[,] dungeon, Room newRoom, Vector2Int coord)
    {
        for (int i = 0; i < newRoom.GetWidth(); i++)
        {
            for (int j = 0; j < newRoom.GetHeight(); j++)
            {
                dungeon[coord.x + i, coord.y + j] = newRoom;

                HookUpExits(dungeon, newRoom, coord + new Vector2Int(i, j));
            }
        }
    }

    private void HookUpExits(Room[,] dungeon, Room r, Vector2Int coord)
    {
        TryHookUpSingleDoor(dungeon, r, coord, coord + Vector2Int.up, Enums.Direction.North);
        TryHookUpSingleDoor(dungeon, r, coord, coord + Vector2Int.down, Enums.Direction.South);
        TryHookUpSingleDoor(dungeon, r, coord, coord + Vector2Int.right, Enums.Direction.East);
        TryHookUpSingleDoor(dungeon, r, coord, coord + Vector2Int.left, Enums.Direction.West);
    }

    // Tries to hook up a door if possible
    private void TryHookUpSingleDoor(Room[,] dungeon, Room r, Vector2Int firstRoomCoord, Vector2Int secondRoomCoord, 
        Enums.Direction dir)
    {
        if (secondRoomCoord.x >= playAreaWidth || secondRoomCoord.y >= playAreaHeight || secondRoomCoord.x < 0 || secondRoomCoord.y < 0)
        {
            return;
        }

        Room secondRoom = dungeon[secondRoomCoord.x, secondRoomCoord.y];
        if (secondRoom != null && secondRoom != r)
        {
            r.SetExitRoom(dir, firstRoomCoord, secondRoom);
            secondRoom.SetExitRoom(GetOppositeDirection(dir), secondRoomCoord, r);
        }
    }

    // Verifies that the selected room can be inserted into the dungeon at the provided coordinate. 
    // This function assumes that the provided coordinate is already empty and as such this function 
    // is meant to verify that larger rooms can properly fit in nearby coordinates of the dungeon
    private bool RoomCanFit(Room[,] dungeon, Room r, Vector2Int coord)
    {
        for (int i = 0; i < r.GetWidth(); i++)
        {
            for (int j = 0; j < r.GetHeight(); j++)
            {
                if (!IsCoordValid(dungeon, coord + new Vector2Int(i, j)))
                {
                    return false;
                }
            }
        }
        return true; 
    }

    // Returns the coordinate obtained when moving by one room size towards the specified 
    // direction when starting at the provided position
    private Vector2Int DirectionToCoord(Enums.Direction dir, Vector2Int currPos)
    {
        switch (dir)
        {
            case Enums.Direction.North:
                return currPos + Vector2Int.up;
            
            case Enums.Direction.South:
                return currPos + Vector2Int.down;

            case Enums.Direction.East:
                return currPos + Vector2Int.right;

            case Enums.Direction.West:
                return currPos + Vector2Int.left;

            default:
                Debug.LogError("Unknown Direction provided to DungeonGenerator.DirectionToCoord");
                return Vector2Int.zero;
        }
    }

    // Verifies if a coordinate is inside the dungeon matrix and that there is no room already defined there
    private bool IsCoordValid(Room[,] dungeon, Vector2Int coord)
    {
        // Index is out of bound and thus is not valid
        if (coord.x < 0 || coord.x >= dungeon.GetLength(0) || coord.y < 0 || coord.y >= dungeon.GetLength(1))
        {
            return false;
        }

        return dungeon[coord.x, coord.y] == null;
    }

    private Enums.Direction GetOppositeDirection(Enums.Direction dir)
    {
        switch (dir)
        {
            case Enums.Direction.North:
                return Enums.Direction.South;

            case Enums.Direction.South:
                return Enums.Direction.North;

            case Enums.Direction.East:
                return Enums.Direction.West;

            case Enums.Direction.West:
                return Enums.Direction.East;

            default:
                Debug.LogError("Unknown Direction provided to DungeonGenerator.GetOppositeDirection");
                return Enums.Direction.NA;
        }
    }

    // Returns the coordinate of room already added to the dungeon by looking around the provided last known position. 
    // The room is selected randomly
    private Vector2Int FindExistingRoom(Room[,] dungeon, Vector2Int lastKnownPosition)
    {
        int searchRange = 3;

        Vector2Int offset = new Vector2Int(UnityEngine.Random.Range(-searchRange, searchRange), 
            UnityEngine.Random.Range(-searchRange, searchRange));

        Vector2Int newPos = lastKnownPosition + offset;
        while (newPos.x < 0 || newPos.x >= dungeon.GetLength(0) || newPos.y < 0 || newPos.y >= dungeon.GetLength(1) 
            || dungeon[newPos.x, newPos.y] == null)
        {
            offset = new Vector2Int(UnityEngine.Random.Range(-searchRange, searchRange),
                UnityEngine.Random.Range(-searchRange, searchRange));
            newPos = lastKnownPosition + offset;
        }

        return newPos;
    }

    // Debug method used to print the dungeon layout to the console
    private void PrintDungeonToConsole(Room[,] dungeon)
    {
        string dungeonStr = "";
        for (int i = 0; i < dungeon.GetLength(0); i++)
        {
            for (int j = 0; j < dungeon.GetLength(1); j++)
            {
                if (dungeon[i, j] == null)
                {
                    dungeonStr += "0";
                }
                else if (dungeon[i, j] == roomsInDungeon[0])
                {
                    dungeonStr += "S";
                }
                else 
                {
                    dungeonStr += "R";
                }
            }
            dungeonStr += "\n";
        }
        //Debug.Log(dungeonStr);
    }

    private void ClearPreviousDungeon()
    {
        DungeonManager.ClearEncounters();
        foreach (Room r in roomsInDungeon)
        {
            Destroy(r.gameObject);
        }
        roomsInDungeon.Clear();
    }

    // Retrieves the next encounter from the list provided. Can return null if the provided list it null. 
    // Assumes that the list of encounters has enough encounter such that the encounter index is a 
    // valid index in the list
    private EncounterData GetNextEncounter(List<EncounterData> encounters)
    {
        EncounterData nextEncounter = null;

        if (encounters != null && encounterIndex < encounters.Count)
        {
            nextEncounter = encounters[encounterIndex];
            encounterIndex++;
        }
        return nextEncounter;
    }
}