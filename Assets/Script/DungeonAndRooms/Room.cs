using System.Collections.Generic;
using UnityEngine;


// Object which will store all the information contained in a room
public class Room : MonoBehaviour
{

    // Width and height are in screen size therefore a room which is 1x1 fits perfectly 
    // on the screen while a 2x1 room would be two times too large to fit on the screen 
    // and the camera will need to move to follow the player around the room 
    [SerializeField]
    private int width = 1;
    [SerializeField]
    private int height = 1;

    [SerializeField]
    private Encounter[] possibleEncounters;

    [SerializeField]
    private Transform enemyTransform;

    private Vector2Int bottomLeftCornerWorldCoord;
    private List<DetectExit> exitPoints = new List<DetectExit>();

    private EncounterData currentEncounter;
    private EncounterData.EnemyData[] enemies;

    [SerializeField]
    private GameObject[] powerUpPrefabList;

    [SerializeField]
    private float powerupDropChance = 0.45f;

    private bool visited = false;

    private void Awake()
    {
        DetectExit[] exits = GetComponentsInChildren<DetectExit>();
        foreach (DetectExit exit in exits)
        {
            exitPoints.Add(exit);
        }
    }


    public int GetWidth() { return width; }
    public int GetHeight() { return height; }

    public void SetExitRoom(Enums.Direction dir, Vector2Int worldCoord, Room r)
    {
        DetectExit exit = FindRoomExit(dir, worldCoord - bottomLeftCornerWorldCoord);
        exit.SetOpenRoom(r);
    }

    public void SetBottomLeftCornerWorldCoord(Vector2Int coord) { bottomLeftCornerWorldCoord = coord; }

    public void SetRandomEncounter()
    {
        Encounter e = possibleEncounters[UnityEngine.Random.Range(0, possibleEncounters.Length)];
        SetEncounter(e.GetData());
    }

    public void SetEncounter(EncounterData e)
    {
        DungeonManager.AddEncounter(e);
        currentEncounter = e;
        enemies = e.GetEnemyData();
        visited = true;
        foreach (EncounterData.EnemyData enemy in e.GetEnemyData())
        {
            if (enemy.health > 0.0f)
            {
                GameObject instantiatedEnemy = Instantiate(PrefabLoader.GetPrefabByName(enemy.prefabName),
                    enemy.GetPosition(), Quaternion.identity, enemyTransform);
                instantiatedEnemy.GetComponent<RectTransform>().anchoredPosition = enemy.GetPosition();
                Entity enemyEntity = instantiatedEnemy.GetComponent<Entity>();
                enemyEntity.SetCurrentHealth(enemy.health);
                enemyEntity.onDamaged += enemy.UpdateHealth;
                visited = false;
            }
        }

        if (!visited)
        {
            RegisterAsListenerForEnemies();
        }
    }

    public void LockRoom()
    {
        if (visited)
        {
            return;
        }

        foreach (DetectExit exit in exitPoints)
        {
            exit.LockDoors(true);
        }
    }

    public void UnlockRoom()
    {
        foreach (DetectExit exit in exitPoints)
        {
            exit.LockDoors(false);
        }

        if(UnityEngine.Random.Range(0.0f, 1.0f) <= powerupDropChance)
        {
            int powerUpIndex = UnityEngine.Random.Range(0, powerUpPrefabList.Length);
            Instantiate(powerUpPrefabList[powerUpIndex], transform);
        }

        visited = true;
    }

    private DetectExit FindRoomExit(Enums.Direction dir, Vector2Int localCoord)
    {
        // TODO check for rooms larger than 1 by 1
        foreach (DetectExit exit in exitPoints)
        {
            if (exit.GetDirection() == dir)
            {
                return exit;
            }
        }

        return null;
    }

    private void CheckEnemyCount(float newHealth)
    {
        if (visited)
        {
            return;
        }

        if (newHealth == 0.0f)
        {
            foreach (EncounterData.EnemyData enemy in enemies)
            {
                if (enemy.health > 0)
                {
                    return;
                }
            }
            UnlockRoom();
        }
    }

    private void RegisterAsListenerForEnemies()
    {
        for (int i = 0; i < enemyTransform.childCount; i++)
        {
            Enemy enemyComponent = enemyTransform.GetChild(i).GetComponentInChildren<Enemy>();

            if (enemyComponent != null)
            {
                enemyComponent.onDamaged += CheckEnemyCount;
            }
        }
    }
}
