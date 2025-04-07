using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Détection du joueur")]
    public LayerMask playerLayer;
    public float visionRange = 5f;
    public Vector2 raycastOffset = new Vector2(0, 0.5f);
    public bool faceRight = true;

    [Header("Patrouille")]
    public Transform platformStart;
    public Transform platformEnd;
    private Vector3 destination;
    public float moveSpeed = 2f;

    [Header("Attaque")]
    public float attackCooldown = 2f;
    private float lastAttackTime;
    public int damageToPlayer = 1;

    [Header("Debug")]
    public bool showRaycast = true;

    [Header("Vie du Slime")]
    [SerializeField] private int health = 100;

    [Header("Effet de recul")]
    public float knockbackForce = 5f;

    private Transform targetPlayer;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool canMove = true;


    void Start()
    {
        destination = platformStart.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (canMove)
            Patrol();

        if (CanSeePlayer() && Time.time > lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }


    public void Damage(int amount)
    {
        health -= amount;
        Debug.Log($"{gameObject.name} a pris {amount} dégâts. Vie restante : {health}");

        Vector2 knockDirection = ((Vector2)transform.position - PlayerPosition()).normalized;
        rb.AddForce(knockDirection * knockbackForce, ForceMode2D.Impulse);

        StartCoroutine(FlashSprite());
        StartCoroutine(DisableMovement(0.5f));


        if (health <= 0)
        {
            Die();
        }
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
            spriteRenderer.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator DisableMovement(float duration)
    {
        canMove = false;
        yield return new WaitForSeconds(duration);
        canMove = true;
    }


    void Patrol()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, destination) < 0.1f)
        {
            if (destination == platformStart.position)
            {
                destination = platformEnd.position;
                Flip();
            }
            else
            {
                destination = platformStart.position;
                Flip();
            }
        }
    }

    void Flip()
    {
        faceRight = !faceRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
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


    Vector2 PlayerPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            return player.transform.position;
        return transform.position;
    }
}