using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;


/*base class for all weapons. Will contain any information 
  pertinant to the weapon such as atk value

  TODO:
  - be able to adjust the attack speed of the weapon and adjust the animation speed
  - get weapon sprite so we can change the weapon being used by Entities
*/

    /*
        TODO: Implement Sprites
    */

public class Weapon : CraftableItem
{
    private WeaponType weaponType;
    private float attack = 5;

    //currently unused paramters
    private float speed = 1;
    private float reach = 1;

    public Weapon(string name, WeaponType type, float attack, List<ResourceType> resourceList) : base(name, ItemType.Weapon, resourceList)
    {
        weaponType = type;
        this.attack = attack;

        ItemSubtypes subtype = (ItemSubtypes)Enum.Parse(typeof(ItemSubtypes), type.ToString());

        this.sprite = ItemManager.GetItemSprite(subtype, level); // Retrieve the sprite from the item manager
    }

    public Weapon(Weapon other) : base(other){
        weaponType = other.weaponType;
        attack = other.attack;
        sprite = other.sprite;
    }

    public override CraftableItem Clone()
    {
        return new Weapon(this);
    }

    public float GetAttack()
    {
        return this.attack;
    }

    public WeaponType GetWeaponType() 
    {
        return weaponType;
    }

    public override String ToString()
    {
        return ( "Type : " + type + ", " + name + ", Level " + level + " " + weaponType + " with " + attack + " attack.");
    }

}