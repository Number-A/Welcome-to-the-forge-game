using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadAreas : MonoBehaviour
{
    public GameObject AreaDoor;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name=="Player")
        {
            if(AreaDoor.name=="Door1")
            {
                SceneManager.LoadScene("Area1");
            }
            else if(AreaDoor.name=="Door2")
            {
                SceneManager.LoadScene("Area2");
            }
            else if(AreaDoor.name=="Door3")
            {
                SceneManager.LoadScene("Area3");
            }
        }
    }
}
