using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer; 
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        float volume;
        audioMixer.GetFloat("MasterVolume", out volume);
        masterSlider.value = Mathf.Pow(10, volume / 20f);
        audioMixer.GetFloat("MusicVolume", out volume);
        musicSlider.value = Mathf.Pow(10, volume / 20f);
        audioMixer.GetFloat("SFXVolume", out volume);
        sfxSlider.value = Mathf.Pow(10, volume / 20f);
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }
}
