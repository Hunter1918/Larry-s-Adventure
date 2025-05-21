using System.Collections;
using UnityEngine;

public class FlyingDiverSlime : MonoBehaviour
{
    [Header("Patrouille")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    [Header("Détection & Plongeon")]
    public float detectionRange = 8f;
    public float diveCooldown = 6f;
    public float diveSpeed = 10f;
    public float groundPauseDuration = 1f;

    [Header("Dégâts")]
    public float contactRadius = 0.5f;
    public int contactDamage = 1;

    [Header("Vie")]
    public int maxHealth = 10;
    private int currentHealth;

    private Vector3 currentTarget;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    private float lastDiveTime;
    private bool isDiving = false;
    private bool playerDetected = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentTarget = pointA.position;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (player == null || isDiving) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // Détection joueur
        if (dist <= detectionRange)
            playerDetected = true;
        else if (playerDetected && dist > detectionRange * 1.5f)
            playerDetected = false;

        if (playerDetected && Time.time >= lastDiveTime + diveCooldown)
        {
            StartCoroutine(DoDiveOnce());
            lastDiveTime = Time.time;
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        Vector2 dir = (currentTarget - transform.position).normalized;
        rb.velocity = dir * moveSpeed;

        if (Vector2.Distance(transform.position, currentTarget) < 0.2f)
            currentTarget = (currentTarget == pointA.position) ? pointB.position : pointA.position;

        if (animator) animator.SetBool("IsFlying", true);
    }

    IEnumerator DoDiveOnce()
    {
        isDiving = true;
        Debug.Log("🧨 Début du dive !");

        if (animator != null)
        {
            animator.ResetTrigger("Fall"); // Sécurité
            animator.SetTrigger("Fall");
            animator.SetBool("IsFlying", false);
            Debug.Log("🎬 Trigger 'Fall' envoyé !");
        }

        rb.velocity = Vector2.down * diveSpeed;

        yield return new WaitUntil(() => IsGrounded());
        rb.velocity = Vector2.zero;

        Debug.Log("🛬 Slime au sol, pause 1s...");
        yield return new WaitForSeconds(groundPauseDuration);

        isDiving = false;
        if (animator) animator.SetBool("IsFlying", true);
        Debug.Log("🔄 Slime reprend la patrouille !");
    }

    bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
            var ph = col.collider.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(contactDamage);
                Debug.Log("💥 Dégâts au joueur par contact !");
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("🩸 Slime prend " + amount + " dégâts (PV restants : " + currentHealth + ")");
        StartCoroutine(FlashSprite());

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator FlashSprite()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            for (int i = 0; i < 2; i++)
            {
                sr.color = new Color(1, 1, 1, 0.3f);
                yield return new WaitForSeconds(0.1f);
                sr.color = Color.white;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    void Die()
    {
        Debug.Log("💀 Slime éliminé !");
        Destroy(gameObject);
    }
}
