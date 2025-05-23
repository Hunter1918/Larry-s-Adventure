using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu_Starting_Game : MonoBehaviour
{
    public string gameSceneName;
    public GameObject Settings;
    public GameObject Credits;

    private void Start()
    {
        Time.timeScale = 1f;
        Settings.SetActive(false);
        Credits.SetActive(false); // Ajout pour propreté
    }

    private void Update()
    {
        if (Credits.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            QuitCreditsMenu();
        }
    }

    public void SettingMainMenu()
    {
        Settings.SetActive(true);
    }

    public void OpenScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitSettingMenu()
    {
        Settings.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void CreditsMainMenu()
    {
        Credits.SetActive(true);
    }

    public void QuitCreditsMenu()
    {
        Credits.SetActive(false);
    }
}