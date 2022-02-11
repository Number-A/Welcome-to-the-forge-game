using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField]
    private Image itemImageUI;

    [SerializeField]
    private GameObject removeButton;

    [SerializeField]
    private Player player;

    private CraftableItem item;

    private void Start()
    {
        RemoveButtonState();
    }

    private void RemoveButtonState(){
        if(itemImageUI.enabled == false)
        {
            removeButton.SetActive(false);
        }else{
            removeButton.SetActive(true);
        }
    }

    public void SetInventory(CraftableItem i)
    {
        item = i;
        SetItemSprite(item.GetSprite());
        RemoveButtonState();
    }

    private void SetItemSprite(Sprite sprite){
        itemImageUI.sprite = sprite;
        itemImageUI.enabled = true;
    }
    public void RemoveItem(){
        if(item == player.GetCurrentArmour()){
            player.SetCurrentArmour(null);
        }else if(item == player.GetCurrentWeapon()){
            player.SetCurrentWeapon(null);
        }
        InventoryManager.RemoveItemFromInventory(item);
        Clear();
        ItemsHeldList.BuildUI();
        RemoveButtonState();
        
    }

    public void EquipItem(){
        if(item !=null){
            if(item.GetItemType().ToString() == "Weapon"){
                if(player.GetCurrentWeapon() == (Weapon)item){
                    player.SetCurrentWeapon(null);
                }else{
                    player.SetCurrentWeapon((Weapon)item);
                }
                WeaponChanger.RefreshWeaponIndex();
            }
            else if(item.GetItemType().ToString() == "Armour")
            {
                if(player.GetCurrentArmour()==(Armour)item){
                    player.SetCurrentArmour(null);
                }else{
                    player.SetCurrentArmour((Armour)item);
                }
            }

        }
    }

    public void Clear()
    {
        itemImageUI.enabled = false;
        item = null;
        RemoveButtonState();
    }
}
