using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// Static class responsible for saving and loading the player data
public class PlayerDataManager 
{
    private static string relativeFilepath = "PlayerData.txt";

    private static PlayerData previousPlayerDataLoaded;

    // for the runIndex, -1 indicates that no run is currently underway while 1, 
    // 2 and 3 indicate a run is currently underway in their respective area
    public static void Save(Player p, int runIndex)
    {
        PlayerData pData = new PlayerData(p, runIndex);
        BinaryFormatter formatter = new BinaryFormatter();
        string filepath = Application.persistentDataPath + relativeFilepath;
        FileStream stream = new FileStream(filepath, FileMode.Create);

        formatter.Serialize(stream, pData);
        stream.Close();
    }

    public static bool HasSaveFile()
    {
        string filepath = Application.persistentDataPath + relativeFilepath;
        return File.Exists(filepath);
    }

    public static void DeleteSaveFile()
    {
        string filepath = Application.persistentDataPath + relativeFilepath;
        if (File.Exists(filepath))
        {
            File.Delete(filepath);
        }
    }
    

    // Can return null if the save file was not found
    public static PlayerData Load()
    {
        string filepath = Application.persistentDataPath + relativeFilepath;
        if (File.Exists(filepath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filepath, FileMode.Open);

            previousPlayerDataLoaded = (PlayerData)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            return null;
        }

        return previousPlayerDataLoaded;
    }

    // Can return null if the save file was not found
    public static PlayerData GetPreviousPlayerDataLoaded()
    {
        return previousPlayerDataLoaded;
    }
}
