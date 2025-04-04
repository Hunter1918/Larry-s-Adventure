using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public string gameSceneName;

    public Button resumeButton;
    public Button settingsButton;
    public Button backFromSettingsButton;

    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);

        resumeButton.onClick.AddListener(Resume);
        settingsButton.onClick.AddListener(OpenSettings);
        backFromSettingsButton.onClick.AddListener(BackToPauseMenu);
    }

    void Update()
{
        //Debug.Log("Update actif");
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Debug.Log("Touche Escape détectée");
            if (settingsMenuUI.activeSelf)
            BackToPauseMenu();
        else if (isPaused)
            Resume();
        else
            Pause();
    }
}


    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    void OpenSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }

    void BackToPauseMenu()
    {
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void ReturnMenu()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
