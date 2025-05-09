using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.UI;

public class Player_Kill : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public CinemachineVirtualCamera virtualCam;
    public Image fadeImage;
    public float fadeDuration = 0.5f;
    public float respawnDelay = 0.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(RespawnPlayer());
        }
    }

    IEnumerator RespawnPlayer()
    {
        yield return StartCoroutine(FadeToBlack());

        yield return new WaitForSeconds(respawnDelay);

        Vector3 checkpointPos = CheckpointManager.Instance.GetLastCheckpointPosition();
        if (checkpointPos != Vector3.zero)
        {
            player.position = checkpointPos;
            if (virtualCam != null)
            {
                virtualCam.Follow = player;
                virtualCam.OnTargetObjectWarped(player, checkpointPos - player.position);
            }
        }
        else
        {
            Debug.LogWarning("Aucun checkpoint défini !");
        }

        yield return StartCoroutine(FadeFromBlack());
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