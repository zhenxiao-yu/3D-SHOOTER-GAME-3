using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OptionScript : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Dropdown resolutionDropDown, qualityDropDown;
    [SerializeField] AudioGetter clickSfx;

    private Resolution[] resolutions;

    void Start()
    {
        // Get available screen resolutions
        resolutions = Screen.resolutions;

        List<string> options = new List<string>();
        int currentScreenResolutionId = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            // Create a string representation of each resolution
            string res = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(res);

            // Check if this resolution matches the current screen resolution
            if (Screen.currentResolution.width == resolutions[i].width &&
                Screen.currentResolution.height == resolutions[i].height)
            {
                currentScreenResolutionId = i;
            }
        }

        // Populate the resolution dropdown with available options
        resolutionDropDown.ClearOptions();
        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = currentScreenResolutionId;

        // Populate the quality dropdown with available options
        qualityDropDown.ClearOptions();
        qualityDropDown.AddOptions(QualitySettings.names.ToList());
        qualityDropDown.value = QualitySettings.GetQualityLevel();

        // Listen for changes in the quality and resolution dropdowns
        qualityDropDown.onValueChanged.AddListener(SetQuality);
        resolutionDropDown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int resolutionId)
    {
        AudioPlayer.Instance.PlaySFX(clickSfx);

        // Set the chosen screen resolution
        Resolution res = resolutions[resolutionId];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityId)
    {
        AudioPlayer.Instance.PlaySFX(clickSfx);

        // Set the chosen graphics quality
        QualitySettings.SetQualityLevel(qualityId);
    }
}
