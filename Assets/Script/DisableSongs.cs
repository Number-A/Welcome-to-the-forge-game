using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This script keeps the "Levels" sound playing as long as you are not in The Forge.
public class DisableSongs : MonoBehaviour
{
    public AudioSource audio;
    public GameObject RoomObject;
    private bool loop=false;
    void Update()
    {
        if(RoomObject.activeInHierarchy)
        {
            EnableSong();
            loop=true;
        }
        if(!RoomObject.activeInHierarchy&&loop)
        {
            DisableSong();
            loop=false;
        }
       
    }
    private void DisableSong()
    {
        audio.Stop();
    }
    private void EnableSong()
    {
        audio.Play();
    }
}
