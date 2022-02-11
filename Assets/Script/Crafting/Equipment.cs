using System;
using System.Collections.Generic;
using static Enums;

/*
    TODO:
        -Implement Sprites
        -Implement Unique Equipment
*/

public class Equipment : CraftableItem
{
    private EquipmentType equipmentType;

    public Equipment(EquipmentType type, string name, List<ResourceType> resourceList) : base(name, ItemType.Weapon, resourceList)
    {
        equipmentType = type;

        ItemSubtypes subtype = (ItemSubtypes)Enum.Parse(typeof(ItemSubtypes), type.ToString());

        this.sprite = ItemManager.GetItemSprite(subtype, level); // Retrieve the sprite from the item manager
    }

    public Equipment(Equipment other) : base(other){
        equipmentType = other.equipmentType;
    }

    public override CraftableItem Clone()
    {
        return new Equipment(this);
    }

    public EquipmentType GetEquipmentType()
    {
        return equipmentType;
    }

    public override String ToString()
    {
        return ("Type : " + type + " " + name + " , Level " + level + " " + equipmentType + ".");
    }
}