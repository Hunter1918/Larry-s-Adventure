using UnityEngine;
using System.Collections;
using DG.Tweening;
public class BossController : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public GameObject meleeHitbox;
    public GameObject projectilePrefab;
    public GameObject PlayerObject;

    [Header("Paramètres de distance")]
    public float meleeRange = 3f;

    [Header("Cooldowns")]
    public float meleeCooldown = 2f;
    public float rangedCooldown = 3f;

    [Header("Vitesse des projectiles")]
    public float projectileSpeed = 8f;

    [Header("Color attaque changed")]
    private SpriteRenderer spriteRenderer;

    [Header("DOtween Animation")]


    private float meleeTimer = 0f;
    private float rangedTimer = 0f;
    private bool isAttacking = false;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
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
        spriteRenderer.color = new Color(1, 1, 1, 0.1f);
        Debug.Log("Boss prépare une attaque de mêlée...");
        yield return new WaitForSeconds(1f);

        meleeHitbox.SetActive(true);
        Debug.Log("Boss frappe !");
        spriteRenderer.color = Color.blue;
        yield return new WaitForSeconds(0.3f);
        //Destroy(PlayerObject);
        meleeHitbox.SetActive(false);

        yield return new WaitForSeconds(3f);
        spriteRenderer.color = Color.red;
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
