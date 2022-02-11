using UnityEngine;


// Temporary script used to damage the player by hitting a key. Used to test the player health HUD
public class TempPlayerDamager : MonoBehaviour
{
    private Player p;

    private void Start()
    {
        p = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            p.TakeDamage(2);
        }
    }
}
