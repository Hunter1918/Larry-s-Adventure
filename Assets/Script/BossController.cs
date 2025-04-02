using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public GameObject meleeHitbox;
    public GameObject projectilePrefab;

    [Header("Paramètres de distance")]
    public float meleeRange = 3f;

    [Header("Cooldowns")]
    public float meleeCooldown = 2f;
    public float rangedCooldown = 3f;

    [Header("Vitesse des projectiles")]
    public float projectileSpeed = 8f;

    private float meleeTimer = 0f;
    private float rangedTimer = 0f;
    private bool isAttacking = false;

    void Update()
    {
        if (isAttacking) return;

        meleeTimer -= Time.deltaTime;
        rangedTimer -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= meleeRange)
        {
            if (meleeTimer <= 0f)
            {
                StartCoroutine(MeleeAttack());
                meleeTimer = meleeCooldown;
            }
        }
        else
        {
            if (rangedTimer <= 0f)
            {
                RangedAttack();
                rangedTimer = rangedCooldown;
            }
        }
    }

    IEnumerator MeleeAttack()
    {
        isAttacking = true;

        Debug.Log("Boss prépare une attaque de mêlée...");
        yield return new WaitForSeconds(0.5f); 

        meleeHitbox.SetActive(true);
        Debug.Log("Boss frappe !");
        yield return new WaitForSeconds(0.3f); 
        meleeHitbox.SetActive(false);

        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    void RangedAttack()
    {
        Debug.Log("Boss lance un projectile ciblé !");

        Vector2 direction = (player.position - transform.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }
    }

}
