using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class TextManager : MonoBehaviour
{
    
    public static Text boneText;
    
    public static Text spiritText;
    
    public static Text stoneText;
    
    public static Text vineText;

    // Start is called before the first frame update
    void Start()
    {
        /*
        boneText = GameObject.Find("BoneText").GetComponent<Text>();
        spiritText = GameObject.Find("SpiritText").GetComponent<Text>();
        stoneText = GameObject.Find("StoneText").GetComponent<Text>();
        vineText = GameObject.Find("VineText").GetComponent<Text>();
        */
        InventoryManager.onResourceChange += UpdateText;
    }

    public static void UpdateText(ResourceType type, int numResources) 
    {
        switch (type) 
        {
            case ResourceType.Bone:
                boneText.text = numResources.ToString();
                break;
            case ResourceType.SpiritEssence:
                spiritText.text = numResources.ToString();
                break;
            case ResourceType.Stone:
                stoneText.text = numResources.ToString();
                break;
            case ResourceType.Vine:
                vineText.text = numResources.ToString();
                break;
        }
    }
}
