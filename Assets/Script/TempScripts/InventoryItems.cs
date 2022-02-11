using UnityEngine;

public class InventoryItems : MonoBehaviour
{
    void Start()
    {
        InventoryManager.AddItemToInventory(Crafting.GetRecipeByIndex(2).Clone());
        InventoryManager.AddItemToInventory(Crafting.GetRecipeByIndex(1).Clone());
        InventoryManager.AddItemToInventory(Crafting.GetRecipeByIndex(3).Clone());
        Debug.Log("Items added to inventory");
    }
}
