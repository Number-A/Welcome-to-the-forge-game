using System.Collections.Generic;
using UnityEngine;

// Primarily used for loading enemy prefabs, allows any part of the code to access 
// the prefabs stored in this singleton by name
public class PrefabLoader : MonoBehaviour
{
    private static PrefabLoader Instance;

    [SerializeField]
    private List<GameObject> prefabs;

    private Dictionary<string, GameObject> prefabsDictionary = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadPrefabs();
        }
    }

    private void LoadPrefabs()
    {
        foreach (GameObject p in prefabs)
        {
            prefabsDictionary.Add(p.name, p);
        }
    }

    public static GameObject GetPrefabByName(string name)
    {
        return Instance.prefabsDictionary[name];
    }
}
