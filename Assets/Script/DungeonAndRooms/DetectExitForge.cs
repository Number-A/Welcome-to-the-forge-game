using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
public class DetectExitForge : MonoBehaviour
{
    [SerializeField]
    private GameObject roomOpen;
    [SerializeField] 
    Direction exitDirection;
    [SerializeField]
    private float spawnOffset = 20.0f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag=="Player")
        {
            transform.parent.parent.gameObject.SetActive(false);
            roomOpen.SetActive(true);
            GameObject oppositeCollider;
            switch (exitDirection) {
                case Direction.North:
                    oppositeCollider = GameObject.Find("SouthCollider");
                    other.transform.position = new Vector3(oppositeCollider.transform.position.x, oppositeCollider.transform.position.y + spawnOffset,0);
                    break;
                case Direction.South:
                    oppositeCollider = GameObject.Find("NorthCollider");
                    other.transform.position = new Vector3(oppositeCollider.transform.position.x, oppositeCollider.transform.position.y -spawnOffset,0);
                    break;
                case Direction.East:
                    oppositeCollider = GameObject.Find("WestCollider");
                    other.transform.position = new Vector3(oppositeCollider.transform.position.x + spawnOffset, oppositeCollider.transform.position.y,0);
                    break;
                case Direction.West:
                    oppositeCollider = GameObject.Find("EastCollider");
                    other.transform.position = new Vector3(oppositeCollider.transform.position.x - spawnOffset, oppositeCollider.transform.position.y,0);
                    break;
                default:
                    throw new System.Exception("ExitCollider does not have a direction set!");
            }
        }
    }


    //public Direction GetDirection() { return exitDirection; }   
}
