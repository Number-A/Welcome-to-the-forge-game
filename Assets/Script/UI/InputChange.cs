using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InputChange : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameOfInputUI;

    [SerializeField]
    private TextMeshProUGUI currentInput;


    private string pairString;

    public void SetInputChange(KeyValuePair<string, KeyCode> keyValuePair)
    {
        pairString = keyValuePair.Key;
        nameOfInputUI.SetText(keyValuePair.Key);
        currentInput.SetText(keyValuePair.Value.ToString());

    }

    public void currentInputButtonClicked(){
        ControlsList.SetCurrKeyName(pairString);
        currentInput.SetText("> Enter Input <");
    }

}
