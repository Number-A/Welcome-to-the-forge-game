using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostSpeed : PowerUp
{
    [SerializeField]
    private float speedBoost;

    protected override void ActivateEffect(Player p, PlayerMovement movement)
    {
        p.AddToSpeedModifier(speedBoost);
    }

    protected override void DeactivateEffect(Player p, PlayerMovement movement)
    {
        p.RemoveFromSpeedModifier(speedBoost);
    }
}
