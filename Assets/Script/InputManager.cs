using System;
using System.Collections.Generic;
using UnityEngine;


//Singleton class which allows keybind changes at runtime
public class InputManager
{
    private static InputManager ManagerInstance = new InputManager();

    // Sends the keybind name of the keybind changed as well as the new key used for that bind
    public static event Action<string, KeyCode> onKeybindChange;

    private Dictionary<string, KeyCode> keybindings = new Dictionary<string, KeyCode>();

    //add or change a keybinding
    public static void Set(string name, KeyCode key)
    {
        ManagerInstance.keybindings[name] = key;
        if (onKeybindChange != null)
        {
            onKeybindChange(name, key);
        }
    }

    //checks if a keybindName has been added to the InputManager
    public static bool Contains(string keybindName) { return ManagerInstance.keybindings.ContainsKey(keybindName); }

    //wrappers for Input.GetKey and Input.GetKeyDown from Unity
    public static bool GetKey(string keybindName) { return Input.GetKey(ManagerInstance.keybindings[keybindName]); }
    public static bool GetKeyDown(string keybindName) 
    { 
        return Input.GetKeyDown(ManagerInstance.keybindings[keybindName]); 
    }

    public static KeyCode GetKeyCode(string keybindName) { return ManagerInstance.keybindings[keybindName]; }

    // Returns a list of the keybindings in the manager
    public static List<KeyValuePair<string, KeyCode>> GetKeybindings()
    {
        List<KeyValuePair<string, KeyCode>> bindings = new List<KeyValuePair<string, KeyCode>>();

        foreach (KeyValuePair<string, KeyCode> pair in ManagerInstance.keybindings)
        {
            bindings.Add(new KeyValuePair<string, KeyCode>(pair.Key, pair.Value));
        }
        return bindings;
    }


    public static void SetDefaultKeys()
    {
        ManagerInstance.SetDefaultKeysImpl();
    }

    public void SetDefaultKeysImpl()
    {
        string[] inputNames = { "Move Up", "Move Down", "Move Left", "Move Right", "Attack", 
            "Toggle Inventory", "Crafting", "Pause" };
        KeyCode[] keyCodes = { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Mouse0, 
            KeyCode.Tab, KeyCode.Mouse0, KeyCode.Escape };

        for (int i = 0; i < inputNames.Length;i++)
        {
            keybindings[inputNames[i]] = keyCodes[i];
        }
    }


    private InputManager() 
    { 
        keybindings = new Dictionary<string, KeyCode>();
        SetDefaultKeysImpl();
    }
}
