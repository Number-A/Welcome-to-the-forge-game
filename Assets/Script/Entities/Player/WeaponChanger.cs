using System;
using UnityEngine;

[RequireComponent(typeof(Player), typeof(PlayerMovement))]
public class WeaponChanger : MonoBehaviour
{
    private static WeaponChanger ChangerInstance = null;

    private Player player;
    private PlayerMovement playerMovement;

    private int currentWeaponIndex;

    // Sends the currently equipped weapon, the previous weapon and the next weapon in that order to 
    // the listeners
    // if there is only one weapon in the inventory the previous and next weapons will be null. 
    // Note that the previous and next weapon can be the same.
    public static event Action<Weapon, Weapon, Weapon> onWeaponChange;

    private void Awake()
    {
        if (ChangerInstance == null)
        {
            ChangerInstance = this;
        }
    }

    public static int GetEquippedWeaponIndex() { return ChangerInstance.currentWeaponIndex; }

    public static void LoadPlayerData(PlayerData data)
    {
        ChangerInstance.currentWeaponIndex = data.inventory.equippedWeaponIndex;
    }

    private void Start()
    {
        if (!InputManager.Contains("ChangeWeaponUp"))
        {
            InputManager.Set("ChangeWeaponUp", KeyCode.E);
            InputManager.Set("ChangeWeaponDown", KeyCode.Q);
        }
        this.player = GetComponent<Player>();
        this.playerMovement = GetComponent<PlayerMovement>();

        InventoryManager.onItemChange += UpdateWeapons;
        currentWeaponIndex = 0;
        if (InventoryManager.GetWeapons().Count > 0)
        {
            Weapon currWeapon = InventoryManager.GetWeaponAtIndex(currentWeaponIndex);
            player.SetCurrentWeapon(currWeapon);
            if (onWeaponChange != null)
            {
                onWeaponChange(currWeapon, GetPreviousWeapon(), GetNextWeapon()); // Send initial update to the event listeners
            }
        }
        else
        {
            if (onWeaponChange != null)
            {
                onWeaponChange(null, null, null); // Send initial update to the event listeners
            }
        }

    }

    private void Update()
    {
        if (InventoryMenuController.IsPaused())
        {
            return;
        }

        if (!this.playerMovement.IsAttacking())
        {
            if (InputManager.GetKeyDown("ChangeWeaponUp"))
            {
                Weapon w = SwitchToNextWeapon();
                player.SetCurrentWeapon(w);
                onWeaponChange(w, GetPreviousWeapon(), GetNextWeapon());
            }
            else if (InputManager.GetKeyDown("ChangeWeaponDown"))
            {
                Weapon w = SwitchToPreviousWeapon();
                player.SetCurrentWeapon(w);
                onWeaponChange(w, GetPreviousWeapon(), GetNextWeapon());
            }
        }
    }

    private Weapon SwitchToNextWeapon()
    {
        //if no weapon don't do anything
        if (InventoryManager.GetWeapons().Count == 0)
        {
            return null;
        }
        
        int weaponIndex = this.currentWeaponIndex;

        if (weaponIndex >= 0)
        {
            if (weaponIndex < InventoryManager.GetWeapons().Count - 1)
            {
                currentWeaponIndex++;
                return InventoryManager.GetWeaponAtIndex(currentWeaponIndex);
            }
            else if (weaponIndex + 1 == InventoryManager.GetWeapons().Count)
            {
                currentWeaponIndex = 0;
                return InventoryManager.GetWeaponAtIndex(currentWeaponIndex);
            }
            else
            {
                RefreshWeaponIndex();
                SwitchToNextWeapon();
            }
        }

        return null;
    }

    private Weapon SwitchToPreviousWeapon()
    {
        //if no weapon don't do anything
        if (InventoryManager.GetWeapons().Count == 0)
        {
            return null;
        }

        int weaponIndex = this.currentWeaponIndex;

        if (weaponIndex >= 0)
        {
            if (weaponIndex < InventoryManager.GetWeapons().Count)
            {
                if (weaponIndex > 0)
                {
                    currentWeaponIndex--;
                    return InventoryManager.GetWeaponAtIndex(currentWeaponIndex);
                }
                else if (weaponIndex == 0)
                {
                    currentWeaponIndex = InventoryManager.GetWeapons().Count - 1;
                    return InventoryManager.GetWeaponAtIndex(currentWeaponIndex);
                }
            }
            else
            {
                RefreshWeaponIndex();
                SwitchToPreviousWeapon();
            }
        }

        return null;
    }

    public static void RefreshWeaponIndex()
    {
        int index = 0;

        foreach (Weapon weapon in InventoryManager.GetWeapons())
        {
            if (ChangerInstance.player.GetCurrentWeapon() == weapon)
            {
                ChangerInstance.currentWeaponIndex = index;
                if(onWeaponChange!=null){
                    onWeaponChange(InventoryManager.GetWeaponAtIndex(ChangerInstance.currentWeaponIndex),
                        ChangerInstance.GetPreviousWeapon(), ChangerInstance.GetNextWeapon());
                }
                return;
            }
            index++;
        }

        ChangerInstance.currentWeaponIndex = 0;
        if (onWeaponChange != null)
        {
            onWeaponChange(InventoryManager.GetWeaponAtIndex(ChangerInstance.currentWeaponIndex),
                ChangerInstance.GetPreviousWeapon(), ChangerInstance.GetNextWeapon());
        }

    } 


    private Weapon GetNextWeapon()
    {
        if (InventoryManager.GetWeapons().Count < 2)
        {
            return null;
        }

        return InventoryManager.GetWeapons()[(this.currentWeaponIndex + 1) % InventoryManager.GetWeapons().Count];
    }

    private Weapon GetPreviousWeapon()
    {
        if (InventoryManager.GetWeapons().Count < 2)
        {
            return null;
        }

        int prevIndex = this.currentWeaponIndex - 1;

        if (prevIndex < 0)
        {
            prevIndex = InventoryManager.GetWeapons().Count - 1;
        }

        return InventoryManager.GetWeapons()[prevIndex];
    }

    private void UpdateWeapons(CraftableItem item, bool added)
    {
        if (item.GetItemType() == Enums.ItemType.Weapon)
        {
            if (!added)
            {
                if (item == player.GetCurrentWeapon())
                {
                    Weapon w = SwitchToNextWeapon();
                    player.SetCurrentWeapon(w);
                }
                // Update HUD
                Weapon currWeapon = InventoryManager.GetWeaponAtIndex(currentWeaponIndex);
                onWeaponChange(currWeapon, GetPreviousWeapon(), GetNextWeapon());
            }
        }
    }
}