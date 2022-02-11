using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public class Crafting : MonoBehaviour
{
    [SerializeField]
    private static List<CraftableItem> recipes = new List<CraftableItem>();

    private static List<Weapon> weaponList = new List<Weapon>();
    private static List<Armour> armourList = new List<Armour>();
    private static List<Equipment> equipmentList = new List<Equipment>();

    //resource cost by level and order (Level 2 Cutlass requires Bone, Stone & Spirit Essence. It would cost 25 Bone, 10 Stone and 5 Spirit Essence)
    [SerializeField]
    private static int[,] weaponCraftingResourceCost = new int[,] { { 10, 5, 0, 0 }, { 25, 10, 5, 0 }, { 75, 30, 15, 10 } };
    private static int[,] armourCraftingResourceCost;

    private float[] weaponAttackByLevel = { 5, 10, 15};
    private float[] armourDefenseByLevel = { 3, 6, 9 };

    public static int armourCraftingCostMultiplier = 3;

    // Start is called before the first frame update
    void Start()
    {
        // Only initialize if we have not previously
        if (recipes.Count > 0)
        {
            return;
        }

        armourCraftingResourceCost = weaponCraftingResourceCost.Clone() as int[,];

        for (int i = 0; i < weaponCraftingResourceCost.GetLength(0); i++)
        {
            for (int j = 0; j < 4; j++)
            {
                armourCraftingResourceCost[i, j] *= armourCraftingCostMultiplier;
            }
        }

        InstantiateRecipes();

        for (int i = 0; i < recipes.Count; i++)
        {
            recipes[i].SetUnlocked(true);
        }
    }

    public static int GetResourceCostByIndexAndType(int levelIndex, int resourceIndex, ItemType item)
    {
        switch (item)
        {
            case ItemType.Weapon:
                return weaponCraftingResourceCost[levelIndex, resourceIndex];
                break;
            case ItemType.Armour:
                return armourCraftingResourceCost[levelIndex, resourceIndex];
                break;
            default: return weaponCraftingResourceCost[levelIndex, resourceIndex];

        }
    }

    private void InstantiateRecipes()
    {
        //weapons 
        recipes.Add(new Weapon("Sword", WeaponType.Sword, weaponAttackByLevel[0], new List<ResourceType> { ResourceType.Bone, ResourceType.SpiritEssence }));
        recipes.Add(new Weapon("Axe", WeaponType.Axe, weaponAttackByLevel[0], new List<ResourceType> { ResourceType.SpiritEssence, ResourceType.Bone }));
        recipes.Add(new Weapon("Spear", WeaponType.Spear, weaponAttackByLevel[0], new List<ResourceType> { ResourceType.Stone, ResourceType.SpiritEssence }));
        recipes.Add(new Weapon("Bow", WeaponType.Bow, weaponAttackByLevel[0], new List<ResourceType> { ResourceType.Vine, ResourceType.Bone }));

        recipes.Add(new Weapon("Cutlass", WeaponType.Sword, weaponAttackByLevel[1], new List<ResourceType> { ResourceType.Bone, ResourceType.Stone, ResourceType.SpiritEssence }));
        recipes.Add(new Weapon("Battleaxe", WeaponType.Axe, weaponAttackByLevel[1], new List<ResourceType> { ResourceType.SpiritEssence, ResourceType.Stone, ResourceType.Bone }));
        recipes.Add(new Weapon("Ranseur", WeaponType.Spear, weaponAttackByLevel[1], new List<ResourceType> { ResourceType.Stone, ResourceType.Vine, ResourceType.SpiritEssence }));
        recipes.Add(new Weapon("Longbow", WeaponType.Bow, weaponAttackByLevel[1], new List<ResourceType> { ResourceType.Vine, ResourceType.SpiritEssence, ResourceType.Bone }));

        recipes.Add(new Weapon("Gladius", WeaponType.Sword, weaponAttackByLevel[2], new List<ResourceType> { ResourceType.Bone, ResourceType.Stone, ResourceType.Vine, ResourceType.SpiritEssence }));
        recipes.Add(new Weapon("Greataxe", WeaponType.Axe, weaponAttackByLevel[2], new List<ResourceType> { ResourceType.SpiritEssence, ResourceType.Stone, ResourceType.Vine, ResourceType.Bone }));
        recipes.Add(new Weapon("Trident", WeaponType.Spear, weaponAttackByLevel[2], new List<ResourceType> { ResourceType.Stone, ResourceType.Vine, ResourceType.Bone, ResourceType.SpiritEssence }));
        recipes.Add(new Weapon("Crossbow", WeaponType.Bow, weaponAttackByLevel[2], new List<ResourceType> { ResourceType.Vine, ResourceType.SpiritEssence, ResourceType.Stone, ResourceType.Bone }));

        //armour
        recipes.Add(new Armour("Mail Shirt", ArmourType.Mail, armourDefenseByLevel[0], new List<ResourceType> { ResourceType.Bone, ResourceType.SpiritEssence }));
        recipes.Add(new Armour("Cloth Robe", ArmourType.Cloth, armourDefenseByLevel[0], new List<ResourceType> { ResourceType.SpiritEssence, ResourceType.Bone }));
        recipes.Add(new Armour("Breastplate", ArmourType.Plate, armourDefenseByLevel[0], new List<ResourceType> { ResourceType.Stone, ResourceType.Bone }));
        recipes.Add(new Armour("Leather Armour", ArmourType.Leather, armourDefenseByLevel[0], new List<ResourceType> { ResourceType.Vine, ResourceType.SpiritEssence }));

        recipes.Add(new Armour("Iron Mail ", ArmourType.Mail, armourDefenseByLevel[1], new List<ResourceType> { ResourceType.Bone, ResourceType.Stone, ResourceType.SpiritEssence }));
        recipes.Add(new Armour("Mage Robe", ArmourType.Cloth, armourDefenseByLevel[1], new List<ResourceType> { ResourceType.SpiritEssence, ResourceType.Vine, ResourceType.Bone }));
        recipes.Add(new Armour("Knight's Plate", ArmourType.Plate, armourDefenseByLevel[1], new List<ResourceType> { ResourceType.Stone, ResourceType.Vine, ResourceType.Bone }));
        recipes.Add(new Armour("Studded Leather", ArmourType.Leather, armourDefenseByLevel[1], new List<ResourceType> { ResourceType.Vine, ResourceType.Stone, ResourceType.SpiritEssence }));

        recipes.Add(new Armour("Fanged Mail ", ArmourType.Mail, armourDefenseByLevel[2], new List<ResourceType> { ResourceType.Bone, ResourceType.Stone, ResourceType.SpiritEssence, ResourceType.Vine }));
        recipes.Add(new Armour("Witch Robe", ArmourType.Cloth, armourDefenseByLevel[2], new List<ResourceType> { ResourceType.SpiritEssence, ResourceType.Vine, ResourceType.Bone, ResourceType.Stone }));
        recipes.Add(new Armour("King's Plate", ArmourType.Plate, armourDefenseByLevel[2], new List<ResourceType> { ResourceType.Stone, ResourceType.Vine, ResourceType.Bone, ResourceType.SpiritEssence }));
        recipes.Add(new Armour("Smith's Apron", ArmourType.Leather, armourDefenseByLevel[2], new List<ResourceType> { ResourceType.Vine, ResourceType.Stone, ResourceType.SpiritEssence, ResourceType.Bone }));
    }

    public static List<CraftableItem> GetRecipes()
    {
        return recipes;
    }
    public static CraftableItem GetRecipeByIndex(int index)
    {
        if (index > 0 && index < recipes.Count)
        {
            return recipes[index];
        }
        return null;
    }

    public static CraftableItem GetRecipeByName(string name) 
    {
        IEnumerable<CraftableItem> recipeQuery =
            from item in recipes
            where item.GetName() == name
            select item;

        return recipeQuery.First();
    }

    public static void CraftItemByIndex(int index)
    {
        CraftableItem item = recipes[index];
        if (CanAfford(item))
        {
            PayRecipeCost(item);
            InventoryManager.AddItemToInventory(item);
        }
    }

    public void CraftItemByName(string name)
    {
        CraftableItem item = GetRecipeByName(name);
        if (CanAfford(item))
        {
            PayRecipeCost(item);
            InventoryManager.AddItemToInventory(item);
        }
        else
        {
            return;
        }
    }

    public static bool CanAfford(CraftableItem item)
    {
        Dictionary<ResourceType, int> resourceInventory = InventoryManager.GetResourceInventory();
        foreach (ResourceType key in resourceInventory.Keys)
        {
            if (resourceInventory[key] < item.GetCostFromType(key))
            {
                return false;
            }
        }

        return true;
    }

    public static void PayRecipeCost(CraftableItem item)
    {
        Dictionary<ResourceType, int> resourceInventory = InventoryManager.GetResourceInventory();

        InventoryManager.RemoveResource(ResourceType.Bone, item.GetCostFromType(ResourceType.Bone));
        InventoryManager.RemoveResource(ResourceType.SpiritEssence, item.GetCostFromType(ResourceType.SpiritEssence));
        InventoryManager.RemoveResource(ResourceType.Stone, item.GetCostFromType(ResourceType.Stone));
        InventoryManager.RemoveResource(ResourceType.Vine, item.GetCostFromType(ResourceType.Vine));
    }
}
