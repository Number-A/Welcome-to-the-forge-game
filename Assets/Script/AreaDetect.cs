using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDetect : MonoBehaviour
{
    public GameObject AreaSongGameObject;
     enum AreaActivated {NA, EnableSong, DisableSong}
     [SerializeField] AreaActivated SongStatus;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name=="Player")
        {
            if(SongStatus==AreaActivated.EnableSong)
            {
                AreaSongGameObject.SetActive(true);
            }
            

            if(SongStatus==AreaActivated.DisableSong)
            {
                AreaSongGameObject.SetActive(false);
            }
        }
    }
}
