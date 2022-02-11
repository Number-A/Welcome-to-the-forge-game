using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoors : MonoBehaviour
{
    public GameObject DoorColliders;
    void Start()
    {
        DoorColliders.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name=="PlayerExitDetector")
        {
            gameObject.SetActive(false);
            DoorColliders.SetActive(false);
            AudioManagerScript.instance.Play("ObtainSound");
        }
    }
}
