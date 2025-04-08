using System.Collections;
using UnityEngine;

public class ExplosiveSlime_TopView : MonoBehaviour
{
    public float detectionRange = 8f;
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
    private SpriteRenderer sr;
    private bool isDying = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
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
            if (ph != null) ph.TakeDamage(contactDamage);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDying) return;
        currentHealth -= amount;
        if (currentHealth <= 0) StartCoroutine(ExplodeAfterFlash());
    }

    IEnumerator ExplodeAfterFlash()
    {
        isDying = true;
        float timer = 0f;
        bool visible = true;

        while (timer < flashDuration)
        {
            sr.enabled = visible;
            visible = !visible;
            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        sr.enabled = true;
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
