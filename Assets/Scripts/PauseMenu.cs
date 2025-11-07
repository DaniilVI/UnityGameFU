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
        Time.timeScale = 1f;
        if (playerMove != null) playerMove.enabled = true;
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        if (playerMove != null) playerMove.enabled = false;
        isPaused = true;
    }

    public void SaveGame()
    {
        Debug.Log("Сохранение прогресса (реализуется позже)");
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
