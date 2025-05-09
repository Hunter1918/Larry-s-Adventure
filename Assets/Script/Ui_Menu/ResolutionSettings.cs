using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ResolutionSettings : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Button applyButton;

    private List<Resolution> availableResolutions = new List<Resolution>();
    private int currentResolutionIndex = 0;

    void Start()
    {
        resolutionDropdown.ClearOptions();
        Resolution[] resolutions = Screen.resolutions;
        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution res = resolutions[i];
            string option = res.width + "x" + res.height;

            if (!options.Contains(option))
            {
                options.Add(option);
                availableResolutions.Add(res);
            }
        }

        resolutionDropdown.AddOptions(options);

        // On v�rifie s'il y a une r�solution sauvegard�e
        if (PlayerPrefs.HasKey("resolutionIndex"))
        {
            currentResolutionIndex = PlayerPrefs.GetInt("resolutionIndex");
        }
        else
        {
            // Sinon on prend la r�solution actuelle
            Resolution current = Screen.currentResolution;
            for (int i = 0; i < availableResolutions.Count; i++)
            {
                if (availableResolutions[i].width == current.width &&
                    availableResolutions[i].height == current.height)
                {
                    currentResolutionIndex = i;
                    break;
                }
            }
        }

        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        applyButton.onClick.AddListener(ApplyResolution);

        // Appliquer automatiquement la r�solution sauvegard�e
        ApplyResolution();
    }

    public void ApplyResolution()
    {
        int index = resolutionDropdown.value;
        Resolution selectedRes = availableResolutions[index];
        Screen.SetResolution(selectedRes.width, selectedRes.height, Screen.fullScreen);

        // Sauvegarder l�index choisi
        PlayerPrefs.SetInt("resolutionIndex", index);
        PlayerPrefs.Save();

        Debug.Log("R�solution appliqu�e et sauvegard�e : " + selectedRes.width + "x" + selectedRes.height);
    }
}