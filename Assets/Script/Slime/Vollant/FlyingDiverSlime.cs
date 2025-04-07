using System.Collections;
using UnityEngine;

public class FlyingDiverSlime : MonoBehaviour
{
    [Header("Mouvements & Détection")]
    public float moveSpeed = 2f;
    public float hoverHeight = 3f;
    public float detectionRange = 8f;

    [Header("Tir")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootCooldown = 3f;
    public int burstCount = 3;
    public float burstInterval = 0.2f;

    [Header("Plongeon")]
    public float diveCooldown = 6f;
    public float diveSpeed = 10f;
    public float stunDuration = 2f;
    public float contactRadius = 0.5f;
    public int contactDamage = 1;

    [Header("Vie")]
    public int maxHealth = 10;
    private int currentHealth;

    private Transform player;
    private float lastShootTime;
    private float lastDiveTime;
    private bool isDiving = false;
    private bool isStunned = false;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (TryGetComponent(out Animator anim))
            animator = anim;
    }

    void Update()
    {
        if (player == null || isDiving || isStunned) return;

        HoverFollowPlayer();

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= detectionRange && Time.time >= lastShootTime + shootCooldown)
        {
            StartCoroutine(ShootBurst());
            lastShootTime = Time.time;
        }

        if (Time.time >= lastDiveTime + diveCooldown)
        {
            StartCoroutine(DoDiveAttack());
            lastDiveTime = Time.time;
        }
    }

    void HoverFollowPlayer()
    {
        Vector2 targetPos = new Vector2(player.position.x, player.position.y + hoverHeight);
        Vector2 moveDir = (targetPos - (Vector2)transform.position).normalized;
        rb.velocity = moveDir * moveSpeed;

        if (animator) animator.SetBool("IsFlying", true);
    }

    IEnumerator ShootBurst()
    {
        for (int i = 0; i < burstCount; i++)
        {
            Vector2 direction = (player.position - firePoint.position).normalized;
            float angleOffset = (i - (burstCount - 1) / 2f) * 10f;
            Quaternion rotation = Quaternion.Euler(0, 0, angleOffset);
            Vector2 spreadDir = rotation * direction;

            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            proj.GetComponent<Projectile>().SetDirection(spreadDir);

            yield return new WaitForSeconds(burstInterval);
        }
    }

    IEnumerator DoDiveAttack()
    {
        isDiving = true;
        rb.velocity = Vector2.zero;
        if (animator) animator.SetTrigger("Dive");

        Vector2 diveDirection = (player.position - transform.position).normalized;
        rb.velocity = diveDirection * diveSpeed;

        float timer = 0f;
        float maxTime = 1.5f;

        while (!IsGrounded() && timer < maxTime)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, contactRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    PlayerHealth ph = hit.GetComponent<PlayerHealth>();
                    if (ph != null)
                    {
                        ph.TakeDamage(contactDamage);
                        Debug.Log("💥 Le slime a touché le joueur en fonçant !");
                    }
                    break;
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        isDiving = false;
        isStunned = true;
        if (animator) animator.SetBool("IsFlying", false);

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        if (animator) animator.SetBool("IsFlying", true);
    }

    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
        return hit.collider != null;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("🩸 Slime volant prend " + amount + " dégâts (restant : " + currentHealth + ")");
        StartCoroutine(FlashSprite());
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    IEnumerator FlashSprite()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            for (int i = 0; i < 3; i++)
            {
                sr.color = new Color(1, 1, 1, 0.3f);
                yield return new WaitForSeconds(0.1f);
                sr.color = Color.white;
                yield return new WaitForSeconds(0.1f);
                sr.color = Color.red;
            }
        }
    }


    void Die()
    {
        Debug.Log("💀 Slime volant mort !");
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerHealth ph = collision.collider.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(contactDamage);
                Debug.Log("⚡ Le slime a infligé des dégâts au joueur par contact !");
            }
        }
    }

}
