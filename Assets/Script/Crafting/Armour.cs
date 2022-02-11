using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

/*
    TODO: Implement Sprites
*/

public class Armour : CraftableItem
{
    private ArmourType armourType;

    //currently unused paramters
    private float defense = 1.0f;

    public Armour(string name, ArmourType type, float defense, List<ResourceType> resourceList) : base(name, ItemType.Armour, resourceList)
    {
        armourType = type;
        this.defense = defense;

        ItemSubtypes subtype = (ItemSubtypes)Enum.Parse(typeof(ItemSubtypes), type.ToString());

        this.sprite = ItemManager.GetItemSprite(subtype, level); // Retrieve the item from the item manager
    }

    public Armour(Armour other) : base(other)
    {
        armourType = other.armourType;
        defense = other.defense;
        sprite = other.sprite;

    }

    public override CraftableItem Clone()
    {
        return new Armour(this);
    }

    public ArmourType GetArmourType()
    {
        return armourType;
    }

    public float GetDefense() 
    {
        return defense;
    }

    public override String ToString()
    {
        return ("Type : " + type + " " + name + " , Level " + level + " " + armourType + ".");
    }
}