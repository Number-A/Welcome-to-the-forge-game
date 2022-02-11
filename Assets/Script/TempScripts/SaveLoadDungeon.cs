using UnityEngine;


// Temporary script used to test the saving and loading mechanisms of the dungeon
public class SaveLoadDungeon : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            DungeonManager.SaveDungeon();
            Debug.Log("Dungeon Saved");
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            if (DungeonManager.LoadDungeon())
            {
                Debug.Log("Dungeon was successfully loaded");
            }
            else
            {
                Debug.Log("Dungeon could not be loaded");
            }
        }
    }
}
