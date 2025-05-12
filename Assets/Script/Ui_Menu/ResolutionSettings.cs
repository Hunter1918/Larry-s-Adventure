using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResolutionSettings : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

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

        // Chargement des préférences sauvegardées
        currentResolutionIndex = PlayerPrefs.GetInt("resolutionIndex", currentResolutionIndex);
        bool isFullscreen = PlayerPrefs.GetInt("fullscreen", Screen.fullScreen ? 1 : 0) == 1;

        resolutionDropdown.value = currentResolutionIndex;
        fullscreenToggle.isOn = isFullscreen;
        resolutionDropdown.RefreshShownValue();

        ApplyResolution(); // Appliquer au démarrage
    }

    public void OnResolutionChange(int index)
    {
        currentResolutionIndex = index;
    }

    public void OnFullscreenToggle(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void ApplyResolution()
    {
        Resolution resolution = resolutions[currentResolutionIndex];
        bool isFullscreen = fullscreenToggle.isOn;
        Screen.SetResolution(resolution.width, resolution.height, isFullscreen);

        // Sauvegarde des préférences
        PlayerPrefs.SetInt("resolutionIndex", currentResolutionIndex);
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}