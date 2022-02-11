using UnityEngine;

public class ItemsHeldList : MonoBehaviour
{
    private static ItemsHeldList Instance;

    [SerializeField]
    private ItemSlot[] itemSlots;
    private int nextItemSlotIndex = 0;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InventoryManager.onItemChange += OnItemChange;
        BuildUI();
    }
private void OnDestroy()
    {
        // Remove from events 
        InventoryManager.onItemChange -= OnItemChange;
    }
    private void AddItemsToUI(CraftableItem item)
    {
        itemSlots[nextItemSlotIndex].SetInventory(item);
        nextItemSlotIndex++;
    }

    public static void BuildUI()
    {
        Instance.ClearUI();


        for (int i = 0; i < InventoryManager.GetArmour().Count; i++)
        {
            Instance.AddItemsToUI(InventoryManager.GetArmourAtIndex(i));
        }
        
        for (int i = 0; i < InventoryManager.GetEquipment().Count; i++)
        {
            Instance.AddItemsToUI(InventoryManager.GetEquipmentAtIndex(i));
        }

        for (int i = 0; i < InventoryManager.GetWeapons().Count; i++)
        {
            Instance.AddItemsToUI(InventoryManager.GetWeaponAtIndex(i));
        }
    }

    public void OnItemChange(CraftableItem item, bool wasAdded)
    {
        BuildUI();
    }

    private void ClearUI()
    {
        foreach (ItemSlot slot in itemSlots)
        {
            slot.Clear();
        }
        nextItemSlotIndex = 0;
    }
}
