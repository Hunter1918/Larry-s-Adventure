using System.Collections;
using UnityEngine;

public class FlyingSlime_TopView : MonoBehaviour
{
    [Header("Comportement volant")]
    public float moveSpeed = 2f;
    public float hoverOffset = 2f;
    public float detectionRange = 8f;

    [Header("Tir à distance")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootCooldown = 3f;
    public int burstCount = 3;
    public float burstInterval = 0.2f;

    [Header("Plongeon")]
    public float diveCooldown = 5f;
    public float diveSpeed = 10f;
    public float stunDuration = 2f;
    public int contactDamage = 2;

    [Header("Vie")]
    public int maxHealth = 10;
    private int currentHealth;

    private Transform player;
    private Rigidbody2D rb;
    private float lastShootTime;
    private float lastDiveTime;
    private bool isDiving = false;
    private bool isStunned = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (player == null || isDiving || isStunned) return;

        HoverFollowPlayer();

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= detectionRange && Time.time >= lastShootTime + shootCooldown)
        {
            StartCoroutine(ShootBurst());
            lastShootTime = Time.time;
        }

        if (Time.time >= lastDiveTime + diveCooldown)
        {
            StartCoroutine(DiveAttack());
            lastDiveTime = Time.time;
        }
    }

    void HoverFollowPlayer()
    {
        Vector2 targetPos = new Vector2(player.position.x, player.position.y + hoverOffset);
        Vector2 moveDir = (targetPos - (Vector2)transform.position).normalized;
        rb.velocity = moveDir * moveSpeed;
    }

    IEnumerator ShootBurst()
    {
        for (int i = 0; i < burstCount; i++)
        {
            Vector2 dir = (player.position - firePoint.position).normalized;
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            proj.GetComponent<Projectile>().SetDirection(dir);
            yield return new WaitForSeconds(burstInterval);
        }
    }

    IEnumerator DiveAttack()
    {
        isDiving = true;
        rb.velocity = Vector2.zero;

        Vector2 diveDir = (player.position - transform.position).normalized;
        rb.velocity = diveDir * diveSpeed;

        float timer = 0f;
        float diveDuration = 1.2f;

        while (timer < diveDuration)
        {
            if (Vector2.Distance(transform.position, player.position) < 1f)
            {
                PlayerHealth ph = player.GetComponent<PlayerHealth>();
                if (ph != null)
                    ph.TakeDamage(contactDamage);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        isDiving = false;
        isStunned = true;
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Flying slime prend " + amount + " dégâts");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("☠️ Slime volant éliminé");
        Destroy(gameObject);
    }
}
