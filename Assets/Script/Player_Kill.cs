using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.UI;

public class Player_Kill : MonoBehaviour
{
    [Header("Références")]
    public Transform player; 
    public Transform respawnPoint; 
    public CinemachineVirtualCamera virtualCam; 

    [Header("Délai avant respawn")]
    public float respawnDelay = 0.5f; 

    [Header("Effet de fondu noir")]
    public Image fadeImage; 
    public float fadeDuration = 0.5f; 

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

        if (player != null && respawnPoint != null)
        {
            player.position = respawnPoint.position;

            if (virtualCam != null)
            {
                virtualCam.Follow = player;
                virtualCam.OnTargetObjectWarped(player, respawnPoint.position - player.position);

            }
        }
        else
        {
            Debug.LogWarning("RespawnZone : Le joueur ou le point de réapparition n'est pas défini !");
        }

        yield return StartCoroutine(FadeFromBlack());
    }

    IEnumerator FadeToBlack()
    {
        if (fadeImage != null)
        {
            float alpha = 0f;
            while (alpha < 1f)
            {
                alpha += Time.deltaTime / fadeDuration;
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
        }
    }

    IEnumerator FadeFromBlack()
    {
        if (fadeImage != null)
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
}