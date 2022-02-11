using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// Stores information about the currently generated dungeon. 
// Is also used to save and load the previously generated dungeon
public class DungeonManager
{
    private static DungeonManager Instance = new DungeonManager();

    // Relative filepath to the file containing the data for the currently saved run
    private static string relativeFilepath = "CurrentRunData.txt"; 

    private int seed;
    private List<EncounterData> encounters = new List<EncounterData>();

    private DungeonManager() { Instance = this; }

    public static void AddEncounter(EncounterData e)
    {
        Instance.encounters.Add(e);
    }

    public static void DeleteRun()
    {
        string filepath = Application.persistentDataPath + relativeFilepath;
        if (File.Exists(filepath))
        {
            File.Delete(filepath);
        }
    }

    public static void ClearEncounters() { Instance.encounters.Clear(); }

    // Returns true if the dungeon could be loaded, false otherwise
    public static bool LoadDungeon()
    {
        string filepath = Application.persistentDataPath + relativeFilepath;
        if (File.Exists(filepath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filepath, FileMode.Open);

            DungeonData data = (DungeonData)formatter.Deserialize(stream);
            stream.Close();

            Instance.seed = data.seed;
            DungeonGenerator.LoadDungeon(data.seed, data.encounters);
        }
        else
        {
            return false;
        }

        return true;
    }

    public static void SaveDungeon()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string filepath = Application.persistentDataPath + relativeFilepath;
        FileStream stream = new FileStream(filepath, FileMode.Create);

        DungeonData saveData = new DungeonData(Instance.seed, Instance.encounters);

        formatter.Serialize(stream, saveData);
        stream.Close();
    }

    public static void SetSeed(int seed)
    {
        Instance.seed = seed;
    }

    [Serializable]
    private struct DungeonData
    {
        public int seed;
        public List<EncounterData> encounters;

        public DungeonData(int seed, List<EncounterData> encounter) 
        {
            this.seed = seed;
            this.encounters = encounter;
        }
    }
}
