using UnityEngine;
using UnityEngine.SceneManagement;

public class FinButtons : MonoBehaviour
{
    [Header("Nom de la scène à relancer")]
    public string sceneToReload = "Game"; // 🔁 Mets ici le nom exact de ta scène de jeu

    public void Rejouer()
    {
        Time.timeScale = 1f; // Juste au cas où t'étais en pause
        SceneManager.LoadScene(sceneToReload);
    }

    public void Quitter()
    {
        Debug.Log("Quitter le jeu");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
