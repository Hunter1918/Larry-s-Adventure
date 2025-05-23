using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player_Kill : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public Image fadeImage;
    public float fadeDuration = 0.5f;
    public float respawnDelay = 0.5f;

    private bool isRespawning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isRespawning)
        {
            StartCoroutine(RespawnPlayer());
        }
        Destroy(other.gameObject);
    }

    IEnumerator RespawnPlayer()
    {
        isRespawning = true;

        yield return StartCoroutine(FadeToBlack());
        yield return new WaitForSeconds(respawnDelay);

        Vector3 checkpointPos = CheckpointManager.Instance.GetLastCheckpointPosition();

        if (checkpointPos != Vector3.zero)
        {
            player.position = checkpointPos;

            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.ResetHealth();
            }
        }
        else
        {
            Debug.LogWarning("Aucun checkpoint défini !");
        }

        yield return StartCoroutine(FadeFromBlack());

        isRespawning = false;
    }

    IEnumerator FadeToBlack()
    {
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    IEnumerator FadeFromBlack()
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}
