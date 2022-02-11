using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceHeldToUI : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI boneText;
    [SerializeField] private TextMeshProUGUI spiritEssenceText;
    [SerializeField] private TextMeshProUGUI stoneText;
    [SerializeField] private TextMeshProUGUI vineText;


    void Awake()
    {
        InventoryManager.onResourceChange += updateText;
         boneText.SetText("{0}",InventoryManager.GetNumResourcesByType(Enums.ResourceType.Bone));
         spiritEssenceText.SetText("{0}", InventoryManager.GetNumResourcesByType(Enums.ResourceType.SpiritEssence));
         stoneText.SetText("{0}", InventoryManager.GetNumResourcesByType(Enums.ResourceType.Stone));
         vineText.SetText("{0}", InventoryManager.GetNumResourcesByType(Enums.ResourceType.Vine));
    }


    private void updateText(Enums.ResourceType type, int count){
        switch (type){
            case Enums.ResourceType.Bone:
                boneText.SetText("{0}",count);
                break;
            case Enums.ResourceType.SpiritEssence:
                spiritEssenceText.SetText("{0}", count);
                break;
            case Enums.ResourceType.Stone:
                stoneText.SetText("{0}", count);
                break;
            case Enums.ResourceType.Vine:
                vineText.SetText("{0}", count);
                break;
        }
        CraftingList.BuildUI();
    }
}
