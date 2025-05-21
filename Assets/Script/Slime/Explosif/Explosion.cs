using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float radius = 2f;
    public int damage = 1;
    public float duration = 0.5f;

    void Start()
    {
        Debug.Log("💥 Explosion instanciée à " + transform.position);
        Debug.Log("🔍 Rayon utilisé pour détection : " + radius);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        if (hits.Length == 0)
        {
            Debug.Log("❌ Aucun objet détecté dans la zone !");
        }

        foreach (Collider2D hit in hits)
        {
            Debug.Log("🧪 Objet détecté : " + hit.name);

            if (hit.CompareTag("Player"))
            {
                Debug.Log("🎯 Le joueur est bien tagué 'Player'");

                PlayerHealth ph = hit.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    Debug.Log("🔥 PlayerHealth trouvé, dégâts infligés !");
                    ph.TakeDamage(damage);
                }
                else
                {
                    Debug.LogWarning("⚠️ Player détecté mais aucun script 'PlayerHealth' trouvé sur " + hit.name);
                }
            }
            else
            {
                Debug.Log("🔸 Objet non joueur détecté : " + hit.name + " (tag : " + hit.tag + ")");
            }
        }

        Destroy(gameObject, duration);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
