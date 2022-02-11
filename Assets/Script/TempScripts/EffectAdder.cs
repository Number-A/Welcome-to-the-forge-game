using UnityEngine;

public class EffectAdder : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement player;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            player.Confuse(2.0f);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            player.Entangle(2.0f);
        }
    }
}
