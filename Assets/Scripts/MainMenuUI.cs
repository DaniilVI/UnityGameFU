using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Основные панели")]
    [SerializeField] private GameObject panelExitConfirm;

    private void Start()
    {
        ShowMainMenu();
    }

    public void OnPlayButton()
    {
        SceneManager.LoadScene("SaveSelect");
    }

    public void OnSettingsButton()
    {
        SceneManager.LoadScene("Settings");
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
        panelExitConfirm.SetActive(false);
    }

    private void ShowExitConfirm()
    {
        panelExitConfirm.SetActive(true);
    }
}
