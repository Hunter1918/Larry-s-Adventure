using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public Slider healthSlider;
    private SpriteRenderer sr;

    private bool isInvincible = false;
    public float invincibilityDuration = 1f;
    [HideInInspector] public bool isChargingAttack = false;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    private Rigidbody2D rb;

    [Header("Références")]
    public Transform player;
    public Transform respawnPoint;
    public CinemachineVirtualCamera virtualCam;

    [Header("Délai avant respawn")]
    public float respawnDelay = 0.5f;

    [Header("Effet de fondu noir")]
    public Image fadeImage;
    public float fadeDuration = 0.5f;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }


    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Camera.main.transform.DOShakePosition(0.2f, 0.3f, 10, 90);
        sr.DOColor(Color.red, 0.05f).SetLoops(2, LoopType.Yoyo);
        /*
        if (!isChargingAttack)
            ApplyKnockback();*/

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(InvincibilityCoroutine());
    }
    /*
    void ApplyKnockback()
    {
        GameObject enemy = GameObject.FindWithTag("Enemy");
        if (enemy != null)
        {
            Vector2 direction = ((Vector2)transform.position - (Vector2)enemy.transform.position).normalized;
            rb.velocity = Vector2.zero;
            rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }
    }*/
    private IEnumerator InvincibilityCoroutine()
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
