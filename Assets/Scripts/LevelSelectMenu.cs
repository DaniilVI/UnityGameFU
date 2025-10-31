using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectMenu : MonoBehaviour
{
    private Button backButton;
    private Transform gridLevels;

    private void Start()
    {
        // Находим объекты внутри Canvas
        Transform panelBackground = transform.Find("LevelMenu");
        if (panelBackground == null)
        {
            Debug.LogError("LevelMenu не найден! Проверь структуру Canvas.");
            return;
        }

        // Ищем кнопку "Назад" и контейнер с уровнями
        backButton = panelBackground.Find("Back")?.GetComponent<Button>();
        gridLevels = panelBackground.Find("Levels");

        if (backButton == null || gridLevels == null)
        {
            Debug.LogError("Back или Levels не найдены! Проверь структуру LevelMenu.");
            return;
        }

        // Подписываем кнопку "Назад"
        backButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));

        // Получаем все кнопки уровней внутри Grid_Levels
        Button[] levelButtons = gridLevels.GetComponentsInChildren<Button>();

        // Назначаем каждой кнопке загрузку соответствующего уровня
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
        SceneManager.LoadScene("LevelTest");
    }
}
