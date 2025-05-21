using UnityEngine;
using System.Collections;

public class ExplosiveSlime : MonoBehaviour
{
    [Header("Détection")]
    public float triggerRadius = 4f;
    public LayerMask playerLayer;
    public float reactionTime = 2f;

    [Header("Explosion")]
    public GameObject explosionPrefab;
    public Animator animator;
    public int health = 1;

    private bool fuseLit = false;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (fuseLit || player == null) return;

        if (Vector2.Distance(transform.position, player.position) <= triggerRadius)
        {
            StartCoroutine(ExplosionRoutine());
        }
    }

    IEnumerator ExplosionRoutine()
    {
        fuseLit = true;
        animator.SetTrigger("Charge");
        yield return new WaitForSeconds(reactionTime);
        animator.SetTrigger("Explode");
        // Explosion sera déclenchée via animation event → ExplodeNow()
    }

    // Appelée depuis l'animation
    public void ExplodeNow()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void TakeDamage(int amount)
    {
        if (fuseLit) return;

        health -= amount;
        if (health <= 0)
        {
            StartCoroutine(ExplosionRoutine());
        }
    }

    // Juste pour le debug dans la scène
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
