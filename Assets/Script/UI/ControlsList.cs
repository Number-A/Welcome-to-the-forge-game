using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlsList : MonoBehaviour
{
    [SerializeField]
    private Transform changeControlUIList;

    [SerializeField]
    private GameObject changeControlUI;


    private static ControlsList Instance;

    private static bool inputsSaved = false;

    private static string currentKeyName;


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (inputsSaved)
        {
            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(vKey))
                {
                    InputManager.Set(currentKeyName, vKey);
                    BuildUI();
                    inputsSaved = false;
                    break;
                }
            }
        }
    }

    public static void SetCurrKeyName(string keyName)
    {
        currentKeyName = keyName;
        inputsSaved = true;
    }


    private void Start()
    {
        BuildUI();
    }

    public static void BuildUI(){
        Instance.ClearUI();
        foreach(KeyValuePair<string, KeyCode> pair in InputManager.GetKeybindings())
        {
            Instance.AddInputChange(pair);
        }
    }

    private void AddInputChange(KeyValuePair<string, KeyCode> pair)
    {
        GameObject inputChangeObject = Instantiate(changeControlUI, transform);
        InputChange inputChange = inputChangeObject.GetComponent<InputChange>();
        inputChange.SetInputChange(pair);
    }

    private void ClearUI()
    {
        foreach (Transform child in changeControlUIList)
        {
            Destroy(child.gameObject);
        }
    }
    
}
