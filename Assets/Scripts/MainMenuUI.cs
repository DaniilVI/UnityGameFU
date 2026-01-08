using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Основные панели")]
    [SerializeField] private GameObject panelExitConfirm;
    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        ShowMainMenu();
        ApplyVolume();
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

    private void ApplyVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat("SFX_VOLUME", 1f);

        if (savedVolume <= 0.0001f)
        {
            audioMixer.SetFloat("SFX_Volume", -80f);
        }
        else
        {
            float dB = Mathf.Log10(savedVolume) * 20f;
            audioMixer.SetFloat("SFX_Volume", dB);
            Debug.Log($"SFX slider={savedVolume}, dB={dB}");
        }
    }
}
