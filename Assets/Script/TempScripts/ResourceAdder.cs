using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceAdder : MonoBehaviour
{
    private void Start()
    {
        InventoryManager.AddResource(Enums.ResourceType.Bone, 30);
        InventoryManager.AddResource(Enums.ResourceType.SpiritEssence, 30);
        InventoryManager.AddResource(Enums.ResourceType.Stone, 30);
        InventoryManager.AddResource(Enums.ResourceType.Vine, 30);

    }
}
