using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectMenu : MonoBehaviour
{
    private Button backButton;
    private Transform gridLevels;

    private void Start()
    {
        Transform panelBackground = transform.Find("LevelMenu");
        if (panelBackground == null)
        {
            Debug.LogError("LevelMenu не найден! Проверь структуру Canvas.");
            return;
        }

        backButton = panelBackground.Find("Back")?.GetComponent<Button>();
        gridLevels = panelBackground.Find("Levels");

        if (backButton == null || gridLevels == null)
        {
            Debug.LogError("Back или Levels не найдены! Проверь структуру LevelMenu.");
            return;
        }

        backButton.onClick.AddListener(() => SceneManager.LoadScene("SaveSelect"));

        Button[] levelButtons = gridLevels.GetComponentsInChildren<Button>();

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
        }
    }

    private void LoadLevel(int levelIndex)
    {
        string sceneName = "Level" + levelIndex;
        Debug.Log($"Загрузка уровня: {sceneName}");
        if (levelIndex == 0)
            SceneManager.LoadScene("LevelTest");
        else 
            SceneManager.LoadScene("LevelTest"); // поменять LevelTest на sceneName чтобы загружать другие уровни
    }
}
