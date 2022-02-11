using UnityEngine;

public class InventoryMenuController : MonoBehaviour
{
    private static InventoryMenuController Instance;

    [SerializeField]
    private GameObject hudObject;
    [SerializeField]
    private GameObject inventoryMenuObj;

    private bool isPaused = false;
    private float oldTimeScale;

    private void Awake()
    {
        Instance = this;
    }

    public static bool IsPaused() { return Instance.isPaused; }

    private void Start()
    {
        if (!InputManager.Contains("Toggle Inventory"))
        {
            InputManager.Set("Toggle Inventory", KeyCode.Tab);
        }

        hudObject.SetActive(true);
        inventoryMenuObj.SetActive(false);
    }

    private void Update()
    {
        if (!PauseMenu.IsPaused())
        {
            if (InputManager.GetKeyDown("Toggle Inventory"))
            {
                ToggleInventory();
            }
        }
    }

    private void ToggleInventory()
    {
        hudObject.SetActive(!hudObject.activeSelf);
        inventoryMenuObj.SetActive(!inventoryMenuObj.activeSelf);

        if (isPaused)
        {
            Time.timeScale = oldTimeScale;
            isPaused = false;
        }
        else
        {
            oldTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;
            isPaused = true;
        }
    }
}
