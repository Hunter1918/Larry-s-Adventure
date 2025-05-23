using UnityEngine;

public class FinalChoiceTrigger : MonoBehaviour
{
    public GameObject choixUI; // Panneau avec le texte + boutons
    public string playerTag = "Player";

    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag(playerTag))
        {
            hasTriggered = true;
            Time.timeScale = 0f; // pause le jeu
            choixUI.SetActive(true);
        }
    }
}
