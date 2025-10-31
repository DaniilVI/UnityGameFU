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
        ShowMainMenu(); // При запуске — главное меню
    }

    // ====== Основное меню ======
    public void OnPlayButton()
    {
        // Пока просто загрузим меню выбора уровня
        SceneManager.LoadScene("LevelSelect"); // Создашь потом сцену LevelSelect
    }

    public void OnSettingsButton()
    {
        ShowSettings();
    }

    public void OnExitButton()
    {
        ShowExitConfirm();
    }

    // ====== Настройки ======
    public void OnBackFromSettings()
    {
        ShowMainMenu();
    }

    // ====== Окно выхода ======
    public void OnConfirmExitYes()
    {
        Debug.Log("Выход из игры...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // для редактора
#endif
    }

    public void OnConfirmExitNo()
    {
        ShowMainMenu();
    }

    // ====== Вспомогательные методы ======
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
