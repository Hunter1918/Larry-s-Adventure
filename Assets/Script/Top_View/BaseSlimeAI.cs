using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BaseSlimeAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    protected Transform player;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected virtual void Update()
    {
        if (player != null)
        {
            MoveTowardsPlayer();
        }
    }

    protected virtual void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

    public virtual void Damage(int amount)
    {
        Die(); // Basique pour test
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
