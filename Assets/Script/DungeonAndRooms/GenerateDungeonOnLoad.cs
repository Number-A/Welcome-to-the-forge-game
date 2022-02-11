using UnityEngine;

public class GenerateDungeonOnLoad : MonoBehaviour
{
    [SerializeField]
    private bool randomize = false;

    private void Start()
    {
        if (SaveManager.IsLoading())
        {
            DungeonManager.LoadDungeon();
            return;
        }

        if (randomize)
        {
           DungeonGenerator.GenerateDungeon();   
        }
        else
        {
            DungeonGenerator.GenerateDungeon(0);
        }
    }
}
