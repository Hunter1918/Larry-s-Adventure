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
    private Vector3 destination;
    public float moveSpeed = 2f;

    [Header("Attaque")]
    public float attackCooldown = 2f;
    private float lastAttackTime;

    [Header("Debug")]
    public bool showRaycast = true;

    private Transform targetPlayer;

    void Start()
    {
        destination = platformStart.position;
    }

    void Update()
    {
        Patrol();

        if (CanSeePlayer() && Time.time > lastAttackTime + attackCooldown)
        {
            Attack();
        }
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
    }
}