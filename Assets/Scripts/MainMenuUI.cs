using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Основные панели")]
    [SerializeField] private GameObject panelMainMenu;
    [SerializeField] private GameObject panelSettings;
    [SerializeField] private GameObject panelExitConfirm;

    private void Start()
    {
        ShowMainMenu();
    }

    public void OnPlayButton()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void OnSettingsButton()
    {
        ShowSettings();
    }

    public void OnExitButton()
    {
        ShowExitConfirm();
    }

    public void OnBackFromSettings()
    {
        ShowMainMenu();
    }

    public void OnConfirmExitYes()
    {
        Debug.Log("Выход из игры...");
        Application.Quit();
    }

    public void OnConfirmExitNo()
    {
        ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        panelMainMenu.SetActive(true);
        panelSettings.SetActive(false);
        panelExitConfirm.SetActive(false);
    }

    private void ShowSettings()
    {
        panelMainMenu.SetActive(false);
        panelSettings.SetActive(true);
        panelExitConfirm.SetActive(false);
    }

    private void ShowExitConfirm()
    {
        panelMainMenu.SetActive(false);
        panelSettings.SetActive(false);
        panelExitConfirm.SetActive(true);
    }
}
