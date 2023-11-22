using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[System.Serializable]
public class VolumeSettings
{
    [SerializeField] private GameObject panel; // The volume settings panel in the UI
    [SerializeField] private Slider sfxSlider; // Slider for SFX volume
    [SerializeField] private Slider bgmSlider; // Slider for BGM volume
    [SerializeField] private AudioMixer mainMixer; // The main audio mixer for adjusting volumes

    private string sfxKey = "SfxKey"; // PlayerPrefs key for SFX volume
    private string bgmKey = "BgmKey"; // PlayerPrefs key for BGM volume

    public GameObject Panel { get => panel; }

    // Initialize the volume settings panel and sliders
    public void Init()
    {
        // Load and set the initial volume settings from PlayerPrefs
        AdjustSfx(PlayerPrefs.GetFloat(sfxKey, 0f));
        AdjustBgm(PlayerPrefs.GetFloat(bgmKey, 0f));

        // Set the slider values to match the loaded settings
        sfxSlider.value = PlayerPrefs.GetFloat(sfxKey, 0f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmKey, 0f);

        // Add listeners to the sliders to adjust audio volumes when they change
        sfxSlider.onValueChanged.AddListener(AdjustSfx);
        bgmSlider.onValueChanged.AddListener(AdjustBgm);
    }

    // Adjust the SFX volume and save the setting to PlayerPrefs
    private void AdjustSfx(float value)
    {
        mainMixer.SetFloat("SfxVolume", value); // Set the SFX volume in the audio mixer
        PlayerPrefs.SetFloat(sfxKey, value); // Save the SFX volume setting
        PlayerPrefs.Save(); // Save PlayerPrefs immediately
    }

    // Adjust the BGM volume and save the setting to PlayerPrefs
    private void AdjustBgm(float value)
    {
        mainMixer.SetFloat("BgmVolume", value); // Set the BGM volume in the audio mixer
        PlayerPrefs.SetFloat(bgmKey, value); // Save the BGM volume setting
        PlayerPrefs.Save(); // Save PlayerPrefs immediately
    }
}

