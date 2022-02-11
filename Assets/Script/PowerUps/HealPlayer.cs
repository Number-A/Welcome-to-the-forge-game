using UnityEngine;

public class HealPlayer : PowerUp
{
    [SerializeField]
    private float healthPoints = 4f;

    private void Awake()
    {
        addToPowerupScreen = false;
    }

    protected override void ActivateEffect(Player p, PlayerMovement pm)
    {
        p.Heal(healthPoints);
        Destroy(gameObject);
    }

    protected override void DeactivateEffect(Player p, PlayerMovement pm) { }
}
