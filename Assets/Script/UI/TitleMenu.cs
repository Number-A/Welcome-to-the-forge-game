using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    [SerializeField] 
    private GameObject mainMenuObject;
    [SerializeField] 
    private GameObject settingsMenuObject;
    [SerializeField] 
    private GameObject controlsMenuObject;
    [SerializeField]
    private GameObject continueButton;
    [SerializeField]
    private GameObject noSaveFilePanel;
    [SerializeField]
    private GameObject deleteSaveFilePanel;
    [SerializeField]
    private GameObject fileDeletedPanel;


    private void Start()
    {
        EnterMainMenu();
    }

    public void StartGame()
    {
        SaveManager.ResetSaveData();
        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {
        SaveManager.Load();
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void CloseNoSaveFilePanel()
    {
        noSaveFilePanel.SetActive(false);
        settingsMenuObject.SetActive(true);
    }

    public void CloseDeleteSavePanel()
    {
        deleteSaveFilePanel.SetActive(false);
        settingsMenuObject.SetActive(true);
    }

    public void CloseFileDeletedPanel()
    {
        fileDeletedPanel.SetActive(false);
        settingsMenuObject.SetActive(true);
    }

    public void DeleteSaveFile()
    {
        SaveManager.DeleteSaveFile();
        deleteSaveFilePanel.SetActive(false);
        fileDeletedPanel.SetActive(true);
    }

    public void OnDeleteSaveClick()
    {
        settingsMenuObject.SetActive(false);
        if (SaveManager.HasSaveFile())
        {
            deleteSaveFilePanel.SetActive(true);
        }
        else
        {
            noSaveFilePanel.SetActive(true);
        }
    }

    public void EnterSettings(){
        mainMenuObject.SetActive(false);
        settingsMenuObject.SetActive(true);
        controlsMenuObject.SetActive(false);
        deleteSaveFilePanel.SetActive(false);
        noSaveFilePanel.SetActive(false);
        fileDeletedPanel.SetActive(false);
    }

    public void EnterMainMenu(){
        continueButton.SetActive(SaveManager.HasSaveFile());
        mainMenuObject.SetActive(true);
        settingsMenuObject.SetActive(false);
        controlsMenuObject.SetActive(false);
        deleteSaveFilePanel.SetActive(false);
        noSaveFilePanel.SetActive(false);
        fileDeletedPanel.SetActive(false);
    }

    public void EnterControlsMenu(){
        mainMenuObject.SetActive(false);
        settingsMenuObject.SetActive(false);
        controlsMenuObject.SetActive(true);
        deleteSaveFilePanel.SetActive(false);
        noSaveFilePanel.SetActive(false);
        fileDeletedPanel.SetActive(false);
    }
}
