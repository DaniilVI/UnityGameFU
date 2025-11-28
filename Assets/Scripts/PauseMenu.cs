using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject exitConfirm;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    private CharacterMove playerMove;

    private bool isPaused = false;
    private bool isSaved = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        exitConfirm.SetActive(false);
        playerMove = FindObjectOfType<CharacterMove>();

        continueButton.onClick.AddListener(Resume);
        retryButton.onClick.AddListener(Retry);
        saveButton.onClick.AddListener(SaveGame);
        exitButton.onClick.AddListener(ExitToMainMenu);
        yesButton.onClick.AddListener(OnConfirmYes);
        noButton.onClick.AddListener(OnConfirmNo);
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
        isSaved = false;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelTest");
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
        isSaved = true;
        Debug.Log("Сохранение прогресса (реализуется позже)");
    }

    public void ExitToMainMenu()
    {
        if (isSaved)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            exitConfirm.SetActive(true);
        }
    }

    private void OnConfirmYes()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnConfirmNo()
    {
        exitConfirm.SetActive(false);
    }
}
