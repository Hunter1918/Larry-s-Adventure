using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vie")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("UI")]
    public Slider healthSlider;
    private SpriteRenderer sr;

    [Header("Invincibilité")]
    private bool isInvincible = false;
    public float invincibilityDuration = 1f;

    [HideInInspector] public bool isChargingAttack = false;
    
    [Header("Références")]
    public Transform player;
    public CinemachineVirtualCamera virtualCam;

    [Header("Fondu et respawn")]
    public Image fadeImage;
    public float fadeDuration = 0.5f;
    public float respawnDelay = 0.5f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (CheckpointManager.Instance != null && CheckpointManager.Instance.GetLastCheckpointPosition() == Vector3.zero)
        {
            CheckpointManager.Instance.SetCheckpoint(player.position);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Camera.main.transform.DOShakePosition(0.2f, 0.3f, 10, 90);
        sr.DOColor(Color.red, 0.05f).SetLoops(2, LoopType.Yoyo);

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(InvincibilityCoroutine());
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        for (int i = 0; i < 5; i++)
        {
            sr.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(invincibilityDuration / 10);
            sr.color = Color.white;
            yield return new WaitForSeconds(invincibilityDuration / 10);
        }

        isInvincible = false;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void Die()
    {
        Debug.Log("Le joueur est mort !");
        StartCoroutine(RespawnPlayer());
    }

    IEnumerator RespawnPlayer()
    {
        yield return StartCoroutine(FadeToBlack());
        yield return new WaitForSeconds(respawnDelay);

        Vector3 checkpointPos = CheckpointManager.Instance.GetLastCheckpointPosition();

        if (checkpointPos != Vector3.zero)
        {
            Vector3 oldPos = player.position;
            player.position = checkpointPos;

            // Réinitialisation correcte de Cinemachine
            if (virtualCam != null)
            {
                virtualCam.Follow = player;
                virtualCam.OnTargetObjectWarped(player, checkpointPos - oldPos);
            }

            // Réinitialisation de la vie
            currentHealth = maxHealth;
            if (healthSlider != null)
                healthSlider.value = currentHealth;
        }
        else
        {
            Debug.LogWarning("Aucun checkpoint défini !");
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
    public void ResetHealth()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

}
