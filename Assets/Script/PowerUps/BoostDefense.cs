using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostDefense : PowerUp
{
    [SerializeField]
    private float defenseBoost;

    protected override void ActivateEffect(Player p, PlayerMovement movement)
    {
        p.AddToDefenseModifier(defenseBoost);
    }

    protected override void DeactivateEffect(Player p, PlayerMovement movement)
    {
        p.RemoveFromDefenseModifier(defenseBoost);
    }
}
