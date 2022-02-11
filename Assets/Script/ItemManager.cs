using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

// This singleton serves as a database for Weapons to fetch their sprites. This class is a 
// MonoBehavior so that the different sprites for the different weapon types can be specified in the editor
public class ItemManager : MonoBehaviour
{
    private static ItemManager ManagerInstance;
    private static int MAX_ITEM_LEVEL = 3;

    [SerializeField]
    private ItemPair[] itemSprites;

    private Dictionary<ItemSubtypes, List<Sprite>> spriteDictionary = new Dictionary<ItemSubtypes, List<Sprite>>();

    private void Awake()
    {
        ManagerInstance = this;
        LoadSpritesInDictionary();
    }

    public static Sprite GetItemSprite(ItemSubtypes type, int level)
    {
        int levelIndex = level - 1;

        List<Sprite> sprites;

        if (!ManagerInstance.spriteDictionary.ContainsKey(type) || levelIndex < 0)
        {
            return null;
        }

        ManagerInstance.spriteDictionary.TryGetValue(type, out sprites);

        if(sprites.Count <= levelIndex)
        {
            return null;
        }
        return sprites[levelIndex];
    }

    private void LoadSpritesInDictionary()
    {
        List<Sprite> sprites = new List<Sprite>();
        for (int i = 0; i < MAX_ITEM_LEVEL; i++)
        {
            sprites.Add(null);
        }

        foreach (ItemPair p in itemSprites)
        {
            int levelIndex = p.level - 1;

            try
            {
                ManagerInstance.spriteDictionary[p.itemType][levelIndex] = p.sprite;
                //Debug.Log(ManagerInstance.spriteDictionary[p.itemType]);
            }
            catch (KeyNotFoundException)
            {
                sprites = new List<Sprite>();
                for (int i = 0; i < MAX_ITEM_LEVEL; i++)
                {
                    sprites.Add(null);
                }

                ManagerInstance.spriteDictionary.Add(p.itemType, sprites);
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log("Level index out of range");
            }
            finally 
            {
                ManagerInstance.spriteDictionary[p.itemType][levelIndex] = p.sprite;
            }
        }
    }

    // Helper struct which is part of the Editor interface to modify which sprite links to which item subtype
    [Serializable]
    private struct ItemPair
    {
        public Sprite sprite;
        public ItemSubtypes itemType;
        public int level;
    }

}
