using UnityEngine;

public class PlatformEnemy : MonoBehaviour
{
    [Header("Détection du joueur")]
    public LayerMask playerLayer;
    public float visionRange = 5f;
    public Vector2 raycastOffset = new Vector2(0, 0.5f);
    public bool faceRight = true;

    [Header("Patrouille")]
    public Transform platformStart;
    public Transform platformEnd;
    public float moveSpeed = 2f;

    [Header("Attaque")]
    public float attackCooldown = 2f;
    private float lastAttackTime;

    [Header("Debug")]
    public bool showRaycast = true;

    private Transform targetPlayer;

    void Update()
    {
        Patrol();

        if (CanSeePlayer() && Time.time > lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    // Déplacement d'un bord à l'autre
    void Patrol()
    {
        float direction = faceRight ? 1f : -1f;
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

        // Si atteint le bord de la plateforme, on se retourne
        if (faceRight && transform.position.x >= platformEnd.position.x)
        {
            Flip();
        }
        else if (!faceRight && transform.position.x <= platformStart.position.x)
        {
            Flip();
        }
    }

    // Inverse la direction de déplacement
    void Flip()
    {
        faceRight = !faceRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    // Vision par Raycast
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

    // Action d'attaque (à personnaliser)
    void Attack()
    {
        Debug.Log($"{gameObject.name} attaque le joueur !");
        lastAttackTime = Time.time;
    }
}