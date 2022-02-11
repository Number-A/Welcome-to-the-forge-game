using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class InventoryManager : MonoBehaviour
{
    /*
     * TODO:
     *      - Change weaponInventory to a List<Weapon> once that PR is merged
     *      - Implement add Weapon Methods
     */

    private static Dictionary<ResourceType, int> resourceInventory = new Dictionary<ResourceType, int>();
    private static List<Weapon> weaponInventory = new List<Weapon>();
    private static List<Equipment> equipmentInventory = new List<Equipment>();
    private static List<Armour> armourInventory = new List<Armour>();

    public static event Action<ResourceType, int> onResourceChange;
    
    public static event Action<CraftableItem, bool> onItemChange; 

    void Awake()
    {
        if (!resourceInventory.ContainsKey(ResourceType.Bone))
        {
            resourceInventory.Add(ResourceType.Bone, 0);
            resourceInventory.Add(ResourceType.SpiritEssence, 0);
            resourceInventory.Add(ResourceType.Stone, 0);
            resourceInventory.Add(ResourceType.Vine, 0);
        }
    }

    private void Start()
    {
        if (onResourceChange != null)
        {
            onResourceChange(ResourceType.Bone, resourceInventory[ResourceType.Bone]);
            onResourceChange(ResourceType.SpiritEssence, resourceInventory[ResourceType.SpiritEssence]);
            onResourceChange(ResourceType.Stone, resourceInventory[ResourceType.Stone]);
            onResourceChange(ResourceType.Vine, resourceInventory[ResourceType.Vine]);
        }
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddResource(ResourceType.Bone, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddResource(ResourceType.SpiritEssence, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AddResource(ResourceType.Stone, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AddResource(ResourceType.Vine, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var item in weaponInventory)
            {
                Debug.Log(item.ToString());
            }
        }
    }

    public static int GetNumResourcesByType(ResourceType type)
    {
        return resourceInventory[type];
    }

    public static void AddResource(ResourceType type, int numResources)
    {
        resourceInventory[type] += numResources;
        if (onResourceChange != null)
        {
            onResourceChange(type, resourceInventory[type]);
        }
    }

    public static void RemoveResource(ResourceType type, int numResources)
    {
        if (numResources <= resourceInventory[type])
        {
            resourceInventory[type] -= numResources;
        }

        if (onResourceChange != null)
        {
            onResourceChange(type, resourceInventory[type]);
        }
    }

    public static void AddItemToInventory(CraftableItem item)
    {
        switch (item.GetItemType())
        {
            case ItemType.Armour:
                armourInventory.Add((Armour)item);
                break;
            case ItemType.Equipment:
                equipmentInventory.Add((Equipment)item);
                break;
            case ItemType.Weapon:
                weaponInventory.Add((Weapon)item);
                break;
        }


        // Check if we have listeners before notifying them
        if (onItemChange != null)
        {
            onItemChange(item, true);
        }
    }

    public static void RemoveItemFromInventory(CraftableItem item)
    {
        switch (item.GetItemType())
        {
            case ItemType.Armour:
                armourInventory.Remove((Armour)item);
                break;
            case ItemType.Equipment:
                equipmentInventory.Remove((Equipment)item);
                break;
            case ItemType.Weapon:
                weaponInventory.Remove((Weapon)item);
                break;
        }

        if (onItemChange != null)
        {
            onItemChange(item, false);
        }
    }

    public static List<Weapon> GetWeapons()
    {
        return weaponInventory;
    }

    public static Weapon GetWeaponAtIndex(int index)
    {
        if (index >= 0 && index < weaponInventory.Count)
        {
            return weaponInventory[index];
        }

        return null;
    }

    public static List<Armour> GetArmour()
    {
        return armourInventory;
    }

    public static Armour GetArmourAtIndex(int index)
    {
        if (index >= 0 && index < armourInventory.Count)
        {
            return armourInventory[index];
        }

        return null;
    }

    public static List<Equipment> GetEquipment()
    {
        return equipmentInventory;
    }

    public static Equipment GetEquipmentAtIndex(int index)
    {
        if (index >= 0 && index < equipmentInventory.Count)
        {
            return equipmentInventory[index];
        }

        return null;
    }

    public static int GetNumResources()
    {
        return resourceInventory.Count;
    }

    public static Dictionary<ResourceType, int> GetResourceInventory() 
    {
        return resourceInventory;
    }


    // Clears the current inventory from everything. Is used when loading the game
    public static void ClearInventory()
    {
        resourceInventory[ResourceType.Bone] = 0;
        resourceInventory[ResourceType.SpiritEssence] = 0;
        resourceInventory[ResourceType.Stone] = 0;
        resourceInventory[ResourceType.Vine] = 0;

        weaponInventory.Clear();
        armourInventory.Clear();
        equipmentInventory.Clear();
    }
}