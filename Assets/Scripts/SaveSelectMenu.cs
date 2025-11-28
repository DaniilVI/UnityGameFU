using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class SavelSelectMenu : MonoBehaviour
{
    private Button backButton;
    private Transform saves;

    [Header("Delete Confirmation Window")]
    public GameObject confirmWindow;
    public TMP_Text confirmText;
    public Button yesButton;
    public Button noButton;
    private int selectedSlotIndex = -1;

    private void Start()
    {
        Transform background = transform.Find("Background");

        backButton = background.Find("Back")?.GetComponent<Button>();
        saves = background.Find("Saves");

        backButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        confirmWindow.SetActive(false);
        SetupButtons();
    }

    private void SetupButtons()
    {
        for (int i = 0; i < saves.childCount; i++)
        {
            int index = i + 1;
            Transform save = saves.GetChild(i);

            Button slot = save.Find("Slot")?.GetComponent<Button>();
            Button delete = save.Find("Delete")?.GetComponent<Button>();

            if (slot != null)
            {
                string saveName = save.name;
                slot.onClick.AddListener(() => LoadSave(saveName));
            }

            if (delete != null)
            {
                delete.onClick.AddListener(() => ShowConfirmWindow(index));
            }
        }
    }

    private void ShowConfirmWindow(int slot)
    {
        selectedSlotIndex = slot;

        confirmText.text = $"{slot}";
        confirmWindow.SetActive(true);

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(OnConfirmYes);
        noButton.onClick.AddListener(OnConfirmNo);
    }

    private void OnConfirmYes()
    {
        confirmWindow.SetActive(false);

        Debug.Log($"Слот {selectedSlotIndex} очищен (пока только лог)");
    }

    private void OnConfirmNo()
    {
        confirmWindow.SetActive(false);
    }

    private void LoadSave(string saveName)
    {
        Debug.Log("Загрузка сохранения: " + saveName);
        SceneManager.LoadScene("LevelSelect");
    }

    private void DeleteSave(string saveName)
    {
        Debug.Log("Удаление сохранения: " + saveName);
    }
}
