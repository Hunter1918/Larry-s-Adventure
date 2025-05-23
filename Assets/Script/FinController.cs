using UnityEngine;
using UnityEngine.SceneManagement;

public class FinController : MonoBehaviour
{
    public GameObject choixUI;

    public void ChoisirReecrire()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scene_Reecriture"); // ← ta scène narrative alternative
    }

    public void ChoisirCombattre()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scene_ASuivre"); // ← écran à suivre
    }
}
