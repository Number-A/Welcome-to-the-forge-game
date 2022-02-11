using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDoorSound : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name=="Player")
        {
            AudioManagerScript.instance.Play("DoorSound2");
        }
    }
}
