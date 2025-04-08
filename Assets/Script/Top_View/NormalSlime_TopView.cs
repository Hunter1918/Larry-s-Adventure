using System.Collections;
using UnityEngine;

public class NormalSlime_TopView : MonoBehaviour
{
    [Header("Comportement")]
    public float moveSpeed = 2f;
    public float detectionRange = 6f;
    public float attackCooldown = 2f;
    public int damageToPlayer = 1;

    [Header("Vie")]
    public int maxHealth = 5;
    private int currentHealth;

    [Header("Knockback")]
    public float knockbackForce = 5f;

    private Transform player;
    private float lastAttackTime;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;

            if (Time.time > lastAttackTime + attackCooldown)
            {
                TryAttack();
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    void TryAttack()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist < 1.5f)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damageToPlayer);
                lastAttackTime = Time.time;
            }
        }
    }

    public void Damage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} prend {amount} dégâts. PV restants : {currentHealth}");

        Vector2 knockDir = ((Vector2)transform.position - (Vector2)player.position).normalized;
        rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);

        StartCoroutine(FlashSprite());

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} est mort !");
        Destroy(gameObject);
    }

    IEnumerator FlashSprite()
    {
        for (int i = 0; i < 3; i++)
        {
            sr.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
        }
        sr.color = Color.white;
    }
}
