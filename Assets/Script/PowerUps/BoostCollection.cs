using UnityEngine;

/**
 * Increase the chance of the player collecting twice as much resources for a short amount of time.
 */
public class BoostCollection : PowerUp
{
    [SerializeField]
    private float doubleCollectionChance;

    protected override void ActivateEffect(Player p, PlayerMovement movement)
    {
        p.AddToDoubleCollectionChance(doubleCollectionChance);
    }

    protected override void DeactivateEffect(Player p, PlayerMovement movement)
    {
        p.RemoveFromDoubleCollectionChance(doubleCollectionChance);
    }

}
