using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private TextMeshProUGUI healthText;

    [SerializeField]
    private ResourceSlot[] resourceSlots;
    private int numberActiveResourceSlots = 0;

    [SerializeField]
    private Sprite boneSprite;
    [SerializeField]
    private Sprite spiritEssenceSprite;
    [SerializeField]
    private Sprite stoneSprite;
    [SerializeField]
    private Sprite vineSprite;

    [SerializeField]
    private Image activePowerUp;
    [SerializeField]
    private TextMeshProUGUI activeTimeLeft;

    [SerializeField]
    private Image equippedWeaponImage;
    [SerializeField]
    private Image previousWeaponImage;
    [SerializeField]
    private TextMeshProUGUI previousKeyText;
    [SerializeField]
    private Image nextWeaponImage;
    [SerializeField]
    private TextMeshProUGUI nextKeyText;
    [SerializeField]
    private TextMeshProUGUI inventoryText;

    private void Awake()
    {
        if (resourceSlots.Length < 4)
        {
            Debug.LogError("HUD needs at least 4 resource slots for the 4 different types of resources");
        }

        InventoryManager.onResourceChange += UpdateResources;
        WeaponChanger.onWeaponChange += UpdateWeaponHUD;
        InputManager.onKeybindChange += UpdateWeaponKeybinds;
        InputManager.onKeybindChange += UpdateInventoryKeybind;
    }

    private void Start()
    {
        if (!InputManager.Contains("Toggle Inventory"))
        {
            InputManager.Set("Toggle Inventory", KeyCode.Escape);
        }

        UpdateInventoryKeybind("Toggle Inventory", InputManager.GetKeyCode("Toggle Inventory"));
        // Update the resources slots when loading the scene
        RebuildResourceSlots();
    }
private void OnDestroy()
    {
        // Remove from events 
        InventoryManager.onResourceChange -= UpdateResources;
        WeaponChanger.onWeaponChange -= UpdateWeaponHUD;
        InputManager.onKeybindChange -= UpdateWeaponKeybinds;
        InputManager.onKeybindChange -= UpdateInventoryKeybind;
    }
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthSlider.value = currentHealth;
        healthText.SetText("{0}/{1}", currentHealth, maxHealth);
    }

    public void SetupHealth(float currentHealth, float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        UpdateHealth(currentHealth, maxHealth);
    }

    private void UpdateResources(Enums.ResourceType type, int newCount)
    {
        // If the new count is 0 we rebuild immediately since we need to remove a resource slot
        if (newCount == 0)
        {
            RebuildResourceSlots();
            return;
        }

        // Check if resource slot already active
        for (int i = 0; i < numberActiveResourceSlots; i++)
        {
            if (resourceSlots[i].GetType() == type)
            {
                resourceSlots[i].text.SetText("{0}", newCount);
                return;
            }
        }

        // If no active resource slots find just rebuild the whole 
        RebuildResourceSlots();
    }

    private void UpdateWeaponHUD(Weapon newWeapon, Weapon prevWeapon, Weapon nextWeapon)
    {
        UpdateWeaponSprite(newWeapon, equippedWeaponImage);
        UpdateWeaponSprite(prevWeapon, previousWeaponImage);
        UpdateWeaponSprite(nextWeapon, nextWeaponImage);
    }

    private void UpdateWeaponSprite(Weapon w, Image i)
    {
        if (w == null)
        {
            i.enabled = false;
            return;
        }

        i.sprite = w.GetSprite();
        i.enabled = true;
    }

    private void UpdateWeaponKeybinds(string name, KeyCode key)
    {
        if (name == "ChangeWeaponUp")
        {
            nextKeyText.SetText(KeyCodeToStr(key));
        }
        else if (name == "ChangeWeaponDown")
        {
            previousKeyText.SetText(KeyCodeToStr(key));
        }
    }

    private void UpdateInventoryKeybind(string name, KeyCode key)
    {
        if (name == "Toggle Inventory")
        {
            inventoryText.SetText("Inventory: " + KeyCodeToStr(key));
        }
    }

    private void RebuildResourceSlots()
    {
        // Reset the number of active resource slot
        numberActiveResourceSlots = 0;

        // For every resource check if we have some and if we do setup a resource slot for it and 
        // update the number of active resource slots
        if (InventoryManager.GetNumResourcesByType(Enums.ResourceType.Bone) > 0)
        {
            SetupResourceSlot(Enums.ResourceType.Bone, boneSprite);
            numberActiveResourceSlots++;
        }

        if (InventoryManager.GetNumResourcesByType(Enums.ResourceType.SpiritEssence) > 0)
        {
            SetupResourceSlot(Enums.ResourceType.SpiritEssence, spiritEssenceSprite);
            numberActiveResourceSlots++;
        }

        if (InventoryManager.GetNumResourcesByType(Enums.ResourceType.Stone) > 0)
        {
            SetupResourceSlot(Enums.ResourceType.Stone, stoneSprite);
            numberActiveResourceSlots++;
        }

        if (InventoryManager.GetNumResourcesByType(Enums.ResourceType.Vine) > 0)
        {
            SetupResourceSlot(Enums.ResourceType.Vine, vineSprite);
            numberActiveResourceSlots++;
        }

        // Deactivate every slot that is not used
        for (int i = numberActiveResourceSlots; i < resourceSlots.Length; i++)
        {
            resourceSlots[i].SetActive(false);
        }
    }

    private void SetupResourceSlot(Enums.ResourceType type, Sprite sprite)
    {
        resourceSlots[numberActiveResourceSlots].SetActive(true);
        resourceSlots[numberActiveResourceSlots].text.SetText("{0}",
            InventoryManager.GetNumResourcesByType(type));
        resourceSlots[numberActiveResourceSlots].SetType(type);
        resourceSlots[numberActiveResourceSlots].image.sprite = sprite;
    }

    // Used so we can easily change specific keys to have a different value than what ToString might return
    private string KeyCodeToStr(KeyCode key)
    {
        return key.ToString();
    }

    // Struct used to pair images and text elements together in the HUD
    [Serializable]
    public struct ResourceSlot
    {
        private Enums.ResourceType type; // Made private so it doesn't appear in the editor
        public Image background;
        public Image image;
        public TextMeshProUGUI text;

        public Enums.ResourceType GetType() { return type; }
        public void SetType(Enums.ResourceType t) { type = t; }

        public void SetActive(bool active)
        {
            image.enabled = active;
            text.enabled = active;
            background.enabled = active;
        }
    }
}
