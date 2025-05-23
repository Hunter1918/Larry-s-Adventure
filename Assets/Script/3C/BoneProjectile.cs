using UnityEngine;

public class BoneProjectile : MonoBehaviour
{
    public int damage = 1;
    public float speed = 10f;
    public float lifetime = 3f;

    private float timer;
    private float moveDirection = 1f;

    // Références à ignorer (ex: joueur, arme, etc.)
    private Collider2D[] collidersToIgnore;

    public void SetDirection(float dir)
    {
        moveDirection = dir;
        if (dir < 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void IgnoreOwnerCollisions(Collider2D[] ownerColliders)
    {
        collidersToIgnore = ownerColliders;
        Collider2D myCol = GetComponent<Collider2D>();

        foreach (var col in collidersToIgnore)
        {
            if (col != null && myCol != null)
                Physics2D.IgnoreCollision(myCol, col);
        }
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime * moveDirection);
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore les collisions avec le lanceur (Larry + arme)
        if (collidersToIgnore != null)
        {
            foreach (var col in collidersToIgnore)
            {
                if (collision == col)
                    return;
            }
        }

        Debug.Log("Hit: " + collision.name);

        if (collision.CompareTag("Player")) return;

        if (collision.TryGetComponent<Enemy>(out var enemy) && enemy != null)
        {
            enemy.Damage(damage);
        }
        else if (collision.TryGetComponent<BossHealth>(out var boss) && boss != null)
        {
            boss.TakeDamage(damage);
        }
        else if (collision.TryGetComponent<ExplosiveEnemy>(out var ex) && ex != null)
        {
            ex.Damage(damage);
        }
        else if (collision.TryGetComponent<FlyingEnemy>(out var fe) && fe != null)
        {
            fe.Damage(damage);
        }

        Destroy(gameObject);
    }
}
