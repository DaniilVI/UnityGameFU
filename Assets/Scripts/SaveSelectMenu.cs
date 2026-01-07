using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SavelSelectMenu : MonoBehaviour
{
    private Button backButton;
    private Button testLevelsButton;
    private Transform saves;

    [Header("Delete Confirmation Window")]
    public GameObject confirmWindow;
    public TMP_Text confirmText;
    public Button yesButton;
    public Button noButton;
    private int selectedSlotIndex = -1;
    private Dictionary<int, int> data;

    private void Start()
    {
        Transform background = transform.Find("Background");

        backButton = background.Find("Back")?.GetComponent<Button>();
        testLevelsButton = background.Find("TESTLEVELS")?.GetComponent<Button>();
        saves = background.Find("Saves");

        backButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        testLevelsButton.onClick.AddListener(() => SceneManager.LoadScene("LevelSelect"));
        confirmWindow.SetActive(false);
        SetupButtons();
    }

    private void SetupButtons()
    {
        data = DataBaseManager.GetNumberLevelForPlayers();

        for (int i = 0; i < saves.childCount; i++)
        {
            int index = i + 1;
            Transform save = saves.GetChild(i);

            Button slot = save.Find("Slot")?.GetComponent<Button>();
            Button delete = save.Find("Delete")?.GetComponent<Button>();

            if (slot != null)
            {
                slot.onClick.AddListener(() => LoadSave(index));

                if (data.ContainsKey(index))
                {
                    Transform child = slot.transform.GetChild(0);
                    child.GetComponent<TMP_Text>().text = $"{data[index]} уровень";
                }
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
        DataBaseManager.setCharacterId(selectedSlotIndex);

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

        Transform save = saves.GetChild(selectedSlotIndex - 1);
        Button slot = save.Find("Slot")?.GetComponent<Button>();

        if (slot != null)
        {
            Transform child = slot.transform.GetChild(0);
            child.GetComponent<TMP_Text>().text = "пустой слот";
        }

        DataBaseManager.DeleteCharacter();
        Debug.Log($"Слот {selectedSlotIndex} очищен");
    }

    private void OnConfirmNo()
    {
        confirmWindow.SetActive(false);
    }

    private void LoadSave(int index)
    {
        data = DataBaseManager.GetNumberLevelForPlayers();

        if (!data.ContainsKey(index))
        {
            DataBaseManager.CreateCharacter(index);
        }

        DataBaseManager.setCharacterId(index);
        LoadLevel.isLoad = true;

        // string sceneName = "Level" + (data.ContainsKey(index) ? data[index] : 1);
        // Debug.Log("Загрузка сохранения: " + sceneName);
        // SceneManager.LoadScene(sceneName);

        if (data.ContainsKey(index))
        {
            string sceneName = "Level" + data[index];
            Debug.Log("Загрузка сцены: " + sceneName);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("Загрузка сцены: IntroVideo");
            SceneManager.LoadScene("IntroVideo");
        }
    }
}
