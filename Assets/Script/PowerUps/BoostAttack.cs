using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
 * Increase the player's attack for a short amount of time.
 */
public class BoostAttack : PowerUp
{
    [SerializeField]
    private float attackBoost;

    protected override void ActivateEffect(Player p, PlayerMovement movement)
    {
        p.AddToAttackModifier(attackBoost);
    }

    protected override void DeactivateEffect(Player p, PlayerMovement movement)
    {
        p.RemoveFromAttackModifier(attackBoost);
    }

}
