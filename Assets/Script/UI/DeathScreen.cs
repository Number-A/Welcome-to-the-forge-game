using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public void ReturnToForge()
    {
        SceneManager.LoadScene(1); // Load forge scene
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0); // Load main menu scene
    }
}
