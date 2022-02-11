using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

//temp script which adds weapons to the player's inventory
public class WeaponLoaderTemp : MonoBehaviour
{
    private void Start()
    {
        Weapon sword = new Weapon("Sword", WeaponType.Sword, 5, new List<ResourceType> { ResourceType.Bone, ResourceType.SpiritEssence });
        Weapon axe = new Weapon("Axe", WeaponType.Axe, 5, new List<ResourceType> { ResourceType.SpiritEssence, ResourceType.Bone });
        Weapon spear = new Weapon("Spear", WeaponType.Spear, 5, new List<ResourceType> { ResourceType.SpiritEssence, ResourceType.Bone });
        Weapon bow = new Weapon("Bow", WeaponType.Bow, 5, new List<ResourceType> { ResourceType.SpiritEssence, ResourceType.Bone });

        InventoryManager.AddItemToInventory(sword);
        InventoryManager.AddItemToInventory(axe);
        InventoryManager.AddItemToInventory(spear);
        InventoryManager.AddItemToInventory(bow);

        Armour a = new Armour("Armour", ArmourType.Cloth, 0.0f, new List<ResourceType>());
        InventoryManager.AddItemToInventory(a);
        GameObject.FindObjectOfType<Player>().SetCurrentArmour(a);
    }
}
