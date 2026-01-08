using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;

    private const string VOLUME_KEY = "SFX_VOLUME";

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);

        volumeSlider.value = savedVolume;
        ApplyVolume(savedVolume);
        Debug.Log("Loaded volume: " + savedVolume);
    }

    public void OnSliderValueChanged(float value)
    {
        ApplyVolume(value);

        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        PlayerPrefs.Save();
        Debug.Log("Saving volume: " + value);
    }

    private void ApplyVolume(float value)
    {
        if (value <= 0.0001f)
        {
            audioMixer.SetFloat("SFX_Volume", -80f);
        }
        else
        {
            float dB = Mathf.Log10(value) * 20f;
            audioMixer.SetFloat("SFX_Volume", dB);
            Debug.Log($"SFX slider={value}, dB={dB}");
        }
    }
}
