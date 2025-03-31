using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu_Starting_Game : MonoBehaviour
{
    public string gameSceneName;
    public GameObject Settings;

    private void Start()
    {
        Settings.SetActive(false);
    }
    public void SettingMainMenu()
    {
        Settings.SetActive(true);
    }

    public void OpenScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

