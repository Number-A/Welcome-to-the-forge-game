using UnityEngine;
using static Enums;

//This script detects the PlayerExitDetector gameObject that is attached to the player and 
//changes room depending on which room he enters (up, down, left, right)
//RoomOpen should indicate which room needs to be displayed and roomClose should always be the room 
//the player is currently in.
public class DetectExit : MonoBehaviour
{
    public GameObject doorSprite;
    public GameObject lockedDoorSprite;
    [SerializeField]
    private GameObject roomOpen;
    [SerializeField] 
    Direction exitDirection;
    [SerializeField]
    private float spawnOffset = 20.0f;

    private Room nextRoom;
    private bool isLocked;

    private void Awake()
    {
        UpdateDoorSprite();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (roomOpen == null || isLocked) 
        { 
            return; 
        }

        if(other.tag=="Player")
        {
            transform.parent.parent.gameObject.SetActive(false);
            roomOpen.SetActive(true);
            GameObject oppositeCollider;
            switch (exitDirection) {
                case Direction.North:
                    oppositeCollider = GameObject.Find("SouthCollider");
                    other.transform.position = new Vector3(oppositeCollider.transform.position.x, oppositeCollider.transform.position.y + spawnOffset,0);
                   AudioManagerScript.instance.Play("DoorSound2");
                    break;
                case Direction.South:
                    oppositeCollider = GameObject.Find("NorthCollider");
                    other.transform.position = new Vector3(oppositeCollider.transform.position.x, oppositeCollider.transform.position.y -spawnOffset,0);
                    AudioManagerScript.instance.Play("DoorSound2");
                    break;
                case Direction.East:
                    oppositeCollider = GameObject.Find("WestCollider");
                    other.transform.position = new Vector3(oppositeCollider.transform.position.x + spawnOffset, oppositeCollider.transform.position.y,0);
                   AudioManagerScript.instance.Play("DoorSound2");
                    break;
                case Direction.West:
                    oppositeCollider = GameObject.Find("EastCollider");
                    other.transform.position = new Vector3(oppositeCollider.transform.position.x - spawnOffset, oppositeCollider.transform.position.y,0);
                   AudioManagerScript.instance.Play("DoorSound2");
                    break;
                default:
                    throw new System.Exception("ExitCollider does not have a direction set!");
            }
            nextRoom.LockRoom();
        }
    }

    public void SetOpenRoom(Room r)
    {
        this.roomOpen = r.gameObject;
        nextRoom = roomOpen.GetComponent<Room>();
        UpdateDoorSprite();
    }

    public Direction GetDirection() { return exitDirection; }

    public void LockDoors(bool locked)
    {
        if (roomOpen == null)
        {
            return;
        }

        isLocked = locked;
        UpdateDoorSprite();
    }

    private void UpdateDoorSprite()
    {
        if (roomOpen == null)
        {
            doorSprite.SetActive(false);
            lockedDoorSprite.SetActive(false);
        }
        else if (isLocked)
        {
            doorSprite.SetActive(false);
            lockedDoorSprite.SetActive(true);
        }
        else
        {
            doorSprite.SetActive(true);
            lockedDoorSprite.SetActive(false);
        }
    }
}
