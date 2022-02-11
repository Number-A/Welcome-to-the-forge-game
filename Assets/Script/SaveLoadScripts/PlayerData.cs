using System;
using System.Collections.Generic;

// Data class storing the necessary information to save and load the player 
[Serializable]
public class PlayerData
{
    // Run data
    public int runIndex = -1; // -1 indicating that no run is started

    // Area unlocked data
    public int lastAreaUnlocked;

    // Resources
    public float playerCurrHealth;
    public float playerMaxHealth;
    public int numBones;
    public int numSpiritEssence;
    public int numStone;
    public int numVines;

    public InventoryData inventory;

    // On instantiation collects the necessary data to save and load the player
    public PlayerData(Player p, int run) 
    {
        playerCurrHealth = p.GetCurrHealth();
        playerMaxHealth = p.GetMaxHealth();
        inventory = new InventoryData(p);
        CollectResourcesData();
        runIndex = run;
        RetrieveAreaIndex();
    }

    private void CollectResourcesData()
    {
        numBones = InventoryManager.GetNumResourcesByType(Enums.ResourceType.Bone);
        numSpiritEssence = InventoryManager.GetNumResourcesByType(Enums.ResourceType.SpiritEssence);
        numStone = InventoryManager.GetNumResourcesByType(Enums.ResourceType.Stone);
        numVines = InventoryManager.GetNumResourcesByType(Enums.ResourceType.Vine);
    }

    private void RetrieveAreaIndex()
    {
        for (int i = 3; i > 1; i--)
        {
            if (AreaDoorController.IsDoorUnlocked(i - 1))
            {
                lastAreaUnlocked = i;
                break;
            }
        }
    }

    [Serializable]
    public class InventoryData
    {
        public int equippedWeaponIndex;
        public int equippedArmourIndex;
        public List<string> itemNames;

        public InventoryData(Player p)
        {
            itemNames = new List<string>();
            CollectItemData();
            CollectEquippedData(p);
        }

        private void CollectItemData()
        {
            foreach(Armour a in InventoryManager.GetArmour())
            {
                itemNames.Add(a.GetName());
            }

            foreach (Weapon w in InventoryManager.GetWeapons())
            {
                itemNames.Add(w.GetName());
            }

            foreach (Equipment e in InventoryManager.GetEquipment())
            {
                itemNames.Add(e.GetName());
            }
        }

        private void CollectEquippedData(Player p)
        {
            if (p.GetCurrentWeapon() == null)
            {
                equippedWeaponIndex = -1; // -1 indicating that no weapon is equipped
            }
            else
            {
                equippedWeaponIndex = WeaponChanger.GetEquippedWeaponIndex();
            }

            equippedArmourIndex = -1; // -1 indicating that no armour is equipped

            List<Armour> armourList = InventoryManager.GetArmour();
            for (int i = 0; i < armourList.Count; i++)
            {
                if (armourList[i] == p.GetCurrentArmour())
                {
                    equippedArmourIndex = i;
                    break;
                }
            }
        }
    }
}
