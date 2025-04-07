using System.Collections;
using UnityEngine;

public class ExplosiveSlime : MonoBehaviour
{
    public float detectionRange = 10f;
    public float projectileCooldown = 2f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int contactDamage = 10;
    public int maxHealth = 7;
    public float flashDuration = 1f;
    public GameObject explosionPrefab;

    private Transform player;
    private float lastShotTime;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private bool isDying = false;

    [Header("Détection du joueur")]
    public LayerMask playerLayer;
    public float visionRange = 5f;
    public Vector2 raycastOffset = new Vector2(0, 0.5f);
    public bool faceRight = true;

    [Header("Debug")]
    public bool showRaycast = true;

    [Header("Attaque")]
    public float attackCooldown = 2f;
    private float lastAttackTime;
    public int damageToPlayer = 1;

    private Transform targetPlayer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        Debug.Log("SLIME INIT: HP = " + currentHealth);
        Debug.Log("SLIME INIT: Player found = " + (player != null));
    }


    void Update()
    {
        if (isDying || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= detectionRange && Time.time >= lastShotTime + projectileCooldown)
        {
            ShootProjectile();
            lastShotTime = Time.time;
        }
        if (CanSeePlayer() && Time.time > lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }
    bool CanSeePlayer()
    {
        Vector2 direction = faceRight ? Vector2.right : Vector2.left;
        Vector2 origin = (Vector2)transform.position + raycastOffset;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, visionRange, playerLayer);

        if (showRaycast)
            Debug.DrawRay(origin, direction * visionRange, Color.red);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            targetPlayer = hit.transform;
            return true;
        }

        return false;
    }

    void ShootProjectile()
    {
        Vector2 direction = (player.position - firePoint.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>().SetDirection(direction);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDying) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(contactDamage);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        Debug.Log("SLIME: TakeDamage " + amount);
        if (isDying) return;
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            StartCoroutine(ExplodeAfterFlash());
        }
    }
    void Attack()
    {
        Debug.Log($"{gameObject.name} attaque le joueur !");
        lastAttackTime = Time.time;

        if (targetPlayer != null)
        {
            PlayerHealth playerHealth = targetPlayer.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToPlayer);
            }
        }
    }

    IEnumerator ExplodeAfterFlash()
    {
        Debug.Log("SLIME: Explosion triggered");
        isDying = true;

        float timer = 0f;
        bool visible = true;

        while (timer < flashDuration)
        {
            spriteRenderer.enabled = visible;
            visible = !visible;
            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        spriteRenderer.enabled = true;
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
