using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Player_Attack : MonoBehaviour
{
    [Header("Hitbox Attacks")]
    public GameObject normalHitbox;
    public GameObject chargedHitbox;

    [Header("Delays")]
    public float normalAttackDelay = 0.3f;
    public float chargedAttackDelay = 0.6f;
    public float chargedHealthCost = 0.2f;

    [Header("Projectile Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float projectileCooldown = 0.5f;

    private float lastProjectileTime = -999f;
    private bool isAttacking = false;

    private PlayerHealth playerHealth;
    private SpriteRenderer sr;
    private PlayerMovement playerMovement;
    private Animator animator;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        sr = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();

        if (normalHitbox != null) normalHitbox.SetActive(true);
        if (chargedHitbox != null) chargedHitbox.SetActive(true);
    }

    void Update()
    {
        if (isAttacking) return;

        if (Input.GetKeyDown(KeyCode.F) && playerMovement.IsGrounded)
            StartCoroutine(DoAttack(normalHitbox, normalAttackDelay, false));

        if (Input.GetKeyDown(KeyCode.G) && playerMovement.IsGrounded)
        {
            float cost = Mathf.Ceil(playerHealth.maxHealth * chargedHealthCost);
            if (playerHealth.GetCurrentHealth() > cost)
            {
                playerHealth.TakeDamage((int)cost);
                StartCoroutine(DoAttack(chargedHitbox, chargedAttackDelay, true));
            }
            else
            {
                Debug.Log("Pas assez de vie pour attaque chargée !");
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
            TryShootProjectile();
    }

    void TryShootProjectile()
    {
        if (Time.time < lastProjectileTime + projectileCooldown)
        {
            Debug.Log("Attaque à distance en cooldown !");
            return;
        }

        int currentHealth = playerHealth.GetCurrentHealth();
        int cost = Mathf.Max(1, Mathf.FloorToInt(currentHealth * 0.01f));

        if (currentHealth - cost <= 0)
        {
            Debug.Log("Trop peu de vie pour tirer un projectile !");
            return;
        }

        playerHealth.TakeDamage(cost);
        ShootProjectile();
        lastProjectileTime = Time.time;
    }

    void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Projectile prefab ou firePoint non assigné !");
            return;
        }

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        float direction = playerMovement.FacingRight ? 1f : -1f;

        BoneProjectile bone = proj.GetComponent<BoneProjectile>();
        if (bone != null)
            bone.SetDirection(direction);
    }
    public void TriggerDamage()
    {
        if (normalHitbox != null && normalHitbox.TryGetComponent(out PlayerMeleeHitbox hitbox))
            hitbox.TriggerDamage();
    }

    IEnumerator DoAttack(GameObject hitboxGO, float delay, bool isCharged)
    {
        isAttacking = true;

        if (animator != null)
            animator.SetTrigger("Attack");

        if (isCharged)
            playerHealth.isChargingAttack = true;

        sr.DOKill();
        //Color flashColor = isCharged ? Color.red : Color.yellow;
        //sr.color = flashColor;
        //sr.DOColor(Color.white, 0.2f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(delay);

        isAttacking = false;

        if (isCharged)
            playerHealth.isChargingAttack = false;
    }

    public bool IsAttacking() => isAttacking;

    public void EndAttack() => isAttacking = false;

    // Animation Events : Appellent ces fonctions pendant l'anim
    public void EnableDamageWindow()
    {
        if (normalHitbox != null && normalHitbox.TryGetComponent(out PlayerMeleeHitbox hitbox))
            hitbox.EnableDamageWindow();
    }

    public void _TriggerDamage()
    {
        if (normalHitbox != null && normalHitbox.TryGetComponent(out PlayerMeleeHitbox hitbox))
            hitbox.TriggerDamage();
    }

    public void DisableDamageWindow()
    {
        if (normalHitbox != null && normalHitbox.TryGetComponent(out PlayerMeleeHitbox hitbox))
            hitbox.DisableDamageWindow();
    }
}
