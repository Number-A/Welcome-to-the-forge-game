using UnityEngine;
using UnityEngine.SceneManagement;

// Static class responsible for handling the saving and loading of the game
public class SaveManager
{
    private static bool isLoading = false;

    public static bool IsLoading() { return isLoading; }

    private static PlayerData prevData;

    public static void Save(Player p, int runIndex)
    {
        PlayerDataManager.Save(p, runIndex);
        if (runIndex != -1)
        {
            DungeonManager.SaveDungeon();
        }
    }

    public static bool HasSaveFile()
    {
        return PlayerDataManager.HasSaveFile();
    }

    public static void DeleteSaveFile()
    {
        PlayerDataManager.DeleteSaveFile();
        DungeonManager.DeleteRun();
    }

    public static void ResetSaveData()
    {
        InventoryManager.ClearInventory();
        AreaDoorController.ResetData();
    }

    public static void Load()
    {
        if (!HasSaveFile())
        {
            return;
        }

        isLoading = true;
        prevData = PlayerDataManager.Load();
        ResetSaveData();
        UpdateAreaDoors();

        // Load items in the inventory
        foreach (string itemName in prevData.inventory.itemNames)
        {
            InventoryManager.AddItemToInventory(Crafting.GetRecipeByName(itemName).Clone());
        }

        // Load resources into the inventory
        InventoryManager.AddResource(Enums.ResourceType.Bone, prevData.numBones);
        InventoryManager.AddResource(Enums.ResourceType.SpiritEssence, prevData.numSpiritEssence);
        InventoryManager.AddResource(Enums.ResourceType.Stone, prevData.numStone);
        InventoryManager.AddResource(Enums.ResourceType.Vine, prevData.numVines);

        Debug.ClearDeveloperConsole();
        Time.timeScale = 1;

        if (prevData.runIndex != -1)
        {
            SceneManager.LoadScene(prevData.runIndex + 1);
        }
        else 
        {
            SceneManager.LoadScene(1);
        }
    }

    public static void StopLoading()
    {
        isLoading = false;
    }

    // Called at the start of the scene if the manager is currently loading the scene
    public static void LoadPlayerData(Player p)
    {
        Time.timeScale = 1;
        p.LoadPlayerData(prevData);
        WeaponChanger.LoadPlayerData(prevData);
    }

    private static void UpdateAreaDoors()
    {
        for (int i = 0; i < prevData.lastAreaUnlocked - 1; i++)
        {
            AreaDoorController.SetBossDefeated(i);
        }
    }
}
