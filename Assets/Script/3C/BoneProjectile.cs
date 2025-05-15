using UnityEngine;

public class BoneProjectile : MonoBehaviour
{
    public int damage = 1;
    public float speed = 10f;
    public float lifetime = 3f;

    private float timer;
    private float moveDirection = 1f;

    public void SetDirection(float dir)
    {
        moveDirection = dir;

        if (dir < 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
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
        if (collision.CompareTag("Player")) return;

        if (collision.TryGetComponent(out Enemy enemy))
        {
            enemy.Damage(damage);
        }
        else if (collision.TryGetComponent(out BossHealth boss))
        {
            boss.TakeDamage(damage);
        }
        else if (collision.TryGetComponent(out ExplosiveEnemy ex))
        {
            ex.Damage(damage);
        }
        else if (collision.TryGetComponent(out FlyingEnemy fe))
        {
            fe.Damage(damage);
        }

        Destroy(gameObject);
    }
}
