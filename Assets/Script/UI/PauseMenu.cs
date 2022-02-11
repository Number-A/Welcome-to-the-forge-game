using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    private static PauseMenu Instance;

    [SerializeField]
    private GameObject[] powerUpPrefab;

    [SerializeField]
    private GameObject pauseMenuObj;

    [SerializeField]
    private Player player;
    [SerializeField]
    private int runIndex;

    [SerializeField]
    private GameObject powerUpPanel;
    [SerializeField]

    private float previousTimeScale;
    private PlayerMovement playerController;

    [SerializeField]
    private PowerUpDisplay[] powerUpDisplays;

    private int powerUpDisplayIndex = 0;

    private void Awake()
    {
        if (powerUpPanel != null)
        {
            powerUpPanel.SetActive(false);
        }

        foreach (PowerUpDisplay p in powerUpDisplays)
        {
            p.SetActive(false);
        }

        Instance = this;

        if (!InputManager.Contains("Pause"))
        {
            InputManager.Set("Pause", KeyCode.Escape);
        }
        pauseMenuObj.SetActive(false);
        playerController = player.gameObject.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (InputManager.GetKeyDown("Pause"))
        {
            TogglePauseMenu();
        }
    }

    public static void SetPowerUp(PowerUp p, int num)
    {
        if (Instance.powerUpPanel == null)
        {
            return;
        }

        if (!Instance.powerUpPanel.activeSelf)
        {
            Instance.powerUpPanel.SetActive(true);
        }

        for (int i = 0; i < Instance.powerUpDisplays.Length; i++)
        {
            if (Instance.powerUpDisplays[i].powerUpType == p.GetType())
            {
                Instance.powerUpDisplays[i].text.SetText("{0}", num);
                return;
            }
        }

        Instance.powerUpDisplays[Instance.powerUpDisplayIndex].SetPowerUpDisplay(p, GetPowerUpSprite(p), num);
        Instance.powerUpDisplays[Instance.powerUpDisplayIndex].SetActive(true);
        Instance.powerUpDisplayIndex++;
    }

    public static bool IsPaused()
    {
        return Instance.pauseMenuObj.activeSelf;
    }

    public void TogglePauseMenu()
    {
        if (!pauseMenuObj.activeSelf)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;
            pauseMenuObj.SetActive(true);
            playerController.enabled = false;
        }
        else
        {
            Time.timeScale = previousTimeScale;
            pauseMenuObj.SetActive(false);
            playerController.enabled = true;
        }
    }

    public void LoadGame()
    {
        SaveManager.Load();
    }

    public void Save()
    {
        SaveManager.Save(player, runIndex);
    }

    public void SaveAndExit()
    {
        Save();
        Time.timeScale = previousTimeScale;
        SceneManager.LoadScene(0);
    }

    [Serializable]
    public struct PowerUpDisplay
    {
        public Type powerUpType;
        public Image sprite;
        public TextMeshProUGUI text;

        public void SetPowerUpDisplay(PowerUp p, Sprite powerupSprite, int count)
        {
            powerUpType = p.GetType();
            sprite.sprite = powerupSprite;
            text.SetText("{0}", count);
        }

        public void SetActive(bool val)
        {
            sprite.gameObject.SetActive(val);
        }
    }

    private static Sprite GetPowerUpSprite(PowerUp p)
    {
        foreach (GameObject prefab in Instance.powerUpPrefab)
        {
            PowerUp power = prefab.GetComponent<PowerUp>();
            if (power.GetType() == p.GetType())
            {
                return prefab.GetComponent<SpriteRenderer>().sprite;
            }
        }

        Debug.LogError("Unknown power up");
        return null;
    }
}
