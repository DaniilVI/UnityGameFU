using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button exitButton;
    private CharacterMove playerMove;

    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        playerMove = FindObjectOfType<CharacterMove>();

        continueButton.onClick.AddListener(Resume);
        saveButton.onClick.AddListener(SaveGame);
        exitButton.onClick.AddListener(ExitToMainMenu);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Возобновляем игру
        if (playerMove != null) playerMove.enabled = true;
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Останавливаем время
        if (playerMove != null) playerMove.enabled = false;
        isPaused = true;
    }

    public void SaveGame()
    {
        Debug.Log("Сохранение прогресса (реализуется позже)");
        // Тут позже добавишь сохранение в БД
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f; // Восстанавливаем время перед сменой сцены
        SceneManager.LoadScene("MainMenu");
    }
}
