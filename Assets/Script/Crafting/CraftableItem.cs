using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class CraftableItem
{
    protected ItemType type;
    protected string name;
    protected int level;
    protected Sprite sprite;

    private Dictionary<ResourceType, int> resourceCosts = new Dictionary<ResourceType, int>();

    private bool unlocked = false;

    public CraftableItem(CraftableItem other)
    {
        name = other.name;
        type = other.type;
        level =  other.level;
        sprite = other.sprite;
        resourceCosts = new Dictionary<ResourceType, int>();

        foreach(KeyValuePair<ResourceType,  int> pair in other.resourceCosts )
        {
            resourceCosts[pair.Key] = pair.Value;
        }
        if (type == ItemType.Armour) 
        {
            foreach (Enums.ResourceType resource in Enum.GetValues(typeof(Enums.ResourceType)))
            {
                if (resourceCosts.ContainsKey(resource))
                {
                    resourceCosts[resource] *= Crafting.armourCraftingCostMultiplier;
                }
            }
        }
    }

    public CraftableItem(string itemName, ItemType type, List<ResourceType> resourceList)
    {
        this.type = type;
        this.name = itemName;

        switch (resourceList.Count)
        {
            case 2:
                level = 1;
                break;
            case 3:
                level = 2;
                break;
            case 4:
                level = 3;
                break;
            default:
                level = 2;
                break;
        }

        int resourceCount = 0;

        foreach (ResourceType resource in resourceList)
        {
            resourceCosts[resource] = Crafting.GetResourceCostByIndexAndType(level - 1, resourceCount, type);
            resourceCount++;
        }
    }

    public virtual CraftableItem Clone(){
        return new CraftableItem(this);
    }

    public ItemType GetItemType() 
    {
        return type;
    }

    public string GetName()
    {
        return name;
    }

    public int GetLevel() 
    {
        return level;
    }

    public Sprite GetSprite() 
    {
        return sprite;
    }

    public Dictionary<ResourceType, int> GetCosts()
    {
        return resourceCosts;
    }

    public int GetCostFromType(ResourceType type)
    {
        int value;
        resourceCosts.TryGetValue(type, out value);
        return value;
    }

    public bool IsUnlocked()
    {
        return unlocked;
    }

    public void SetUnlocked(bool isUnlocked)
    {
        unlocked = isUnlocked;
    }

    public override String ToString()
    {
        return (unlocked ? "Unlocked: " : "Locked: ") + "Recipe: Level " + level + " " + name + ", it costs " + GetCostFromType(ResourceType.Bone) + " bones, " + GetCostFromType(ResourceType.SpiritEssence) + " spiritEssence, " + GetCostFromType(ResourceType.Stone) + " stone and " + GetCostFromType(ResourceType.Vine) + " vines.";
    }
}