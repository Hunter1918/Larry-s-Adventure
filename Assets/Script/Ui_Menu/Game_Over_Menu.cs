using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject GameOverMenuUI;
    public string mainMenuSceneName;
    public string gameSceneName;

    void Start()
    {
        GameOverMenuUI.SetActive(false);
    }

    public void OpenDeathMenu()
    {
        GameOverMenuUI.SetActive(true); 
    }

    public void CloseDeathMenu()
    {
        GameOverMenuUI.SetActive(false); 
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName); 
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(gameSceneName); 
    }
}