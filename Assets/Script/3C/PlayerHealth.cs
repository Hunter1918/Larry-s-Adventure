using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vie")]
    public int maxHealth = 20;
    private int currentHealth;

    [Header("UI Cœurs")]
    public Transform heartContainer;
    public Image heartPrefab;
    public List<Sprite> heartSprites; // 11 sprites : 0/10 -> 10/10
    private List<Image> heartImages = new List<Image>();

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
    private SpriteRenderer sr;
    [HideInInspector] public bool isInRareMode = false;

    [Header("Animator Controllers")]
    public RuntimeAnimatorController normalController;
    public RuntimeAnimatorController rareController;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();


        InitHearts();

        if (CheckpointManager.Instance != null && player != null && CheckpointManager.Instance.GetLastCheckpointPosition() == Vector3.zero)
        {
            CheckpointManager.Instance.SetCheckpoint(player.position);
        }

    }

    void InitHearts()
    {
        int heartCount = maxHealth / 10; // Ici 2 cœurs
        for (int i = 0; i < heartCount; i++)
        {
            Image heart = Instantiate(heartPrefab, heartContainer);
            heartImages.Add(heart);
        }

        UpdateHearts();
    }

    void UpdateHearts()
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            int startValue = i * 10;
            int heartValue = Mathf.Clamp(currentHealth - startValue, 0, 10);
            heartImages[i].sprite = heartSprites[heartValue];
        }
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Camera.main.transform.DOShakePosition(0.2f, 0.3f, 10, 90);
        sr.DOColor(Color.red, 0.05f).SetLoops(2, LoopType.Yoyo);

        UpdateHearts();

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

    public void Die()
    {
        Debug.Log("Le joueur est mort !");
        StartCoroutine(RespawnPlayer());
    }

    IEnumerator RespawnPlayer()
    {

        yield return StartCoroutine(FadeToBlack());
        yield return new WaitForSeconds(respawnDelay);

        if (CheckpointManager.Instance == null)
        {
            Debug.LogWarning("❌ CheckpointManager.Instance est null !");
            yield return StartCoroutine(FadeFromBlack());
            yield break;
        }

        Vector3 checkpointPos = CheckpointManager.Instance.GetLastCheckpointPosition();

        if (checkpointPos != Vector3.zero)
        {
            if (player == null)
            {
                Debug.LogError("❌ Transform 'player' non assigné dans PlayerHealth !");
                yield break;
            }

            Vector3 oldPos = player.position;
            player.position = checkpointPos;

            if (virtualCam != null)
            {
                virtualCam.Follow = player;
                virtualCam.OnTargetObjectWarped(player, checkpointPos - oldPos);
            }

            currentHealth = maxHealth;
            UpdateHearts();
        }
        else
        {
            Debug.LogWarning("⚠️ Aucun checkpoint défini !");
        }

        yield return StartCoroutine(FadeFromBlack());
        if (animator != null)
        {
            animator.runtimeAnimatorController = isInRareMode ? rareController : normalController;
        }

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
        UpdateHearts();
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHearts();
    }
}
