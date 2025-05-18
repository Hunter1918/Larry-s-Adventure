using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResolutionSettings : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider volumeSlider;

    private Resolution[] resolutions;
    private int currentResolutionIndex;

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        int savedWidth = PlayerPrefs.GetInt("resolutionWidth", Screen.currentResolution.width);
        int savedHeight = PlayerPrefs.GetInt("resolutionHeight", Screen.currentResolution.height);
        bool isFullscreen = PlayerPrefs.GetInt("fullscreen", Screen.fullScreen ? 1 : 0) == 1;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == savedWidth && resolutions[i].height == savedHeight)
            {
                currentResolutionIndex = i;
                break;
            }
        }

        resolutionDropdown.value = currentResolutionIndex;
        fullscreenToggle.isOn = isFullscreen;
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 1f);
        resolutionDropdown.RefreshShownValue();
    }

    public void ApplySettings()
    {
        Resolution resolution = resolutions[resolutionDropdown.value];
        bool isFullscreen = fullscreenToggle.isOn;
        float volume = volumeSlider.value;

        Screen.SetResolution(resolution.width, resolution.height, isFullscreen);
        AudioListener.volume = volume;

        if (ResolutionManager.Instance != null)
            ResolutionManager.Instance.SaveResolution(resolution.width, resolution.height, isFullscreen);

        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.Save();
    }
}