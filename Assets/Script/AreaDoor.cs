using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDoor : MonoBehaviour
{
    [SerializeField]
    private int areaDoorIndex;

    private void OnEnable()
    {
        if (AreaDoorController.IsDoorUnlocked(areaDoorIndex))
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }


}
