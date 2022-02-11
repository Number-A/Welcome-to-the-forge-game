using System.Collections;
using UnityEngine;

// Script responsible for calling the LoadPlayerData function from the SaveManager whenever 
// the manager is loading up the scene
public class LoadSceneReceiver : MonoBehaviour
{
    [SerializeField]
    private Player player;

    private void Start()
    {
        Time.timeScale = 1;
        if (SaveManager.IsLoading())
        {
            SaveManager.LoadPlayerData(player);
            StartCoroutine("StopLoading");
        }
    }

    private IEnumerator StopLoading()
    {
        yield return new WaitForSeconds(0.2f);
        SaveManager.StopLoading();
    }
}
