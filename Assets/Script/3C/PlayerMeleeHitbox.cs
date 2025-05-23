using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeHitbox : MonoBehaviour
{
    private List<Collider2D> enemiesInZone = new();
    private Collider2D col;
    private bool canDealDamage = false;
    private bool hasHit = false;

    public int damage = 2;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        if (col == null)
            Debug.LogError("❌ Aucun Collider2D trouvé sur " + gameObject.name);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Enemy") || other.CompareTag("Boss")) && !enemiesInZone.Contains(other))
        {
            enemiesInZone.Add(other);
            Debug.Log("🎯 Ennemi détecté : " + other.name);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (enemiesInZone.Contains(other))
        {
            enemiesInZone.Remove(other);
            Debug.Log("↩️ Ennemi quitté : " + other.name);
        }
    }

    public void EnableDamageWindow()
    {
        canDealDamage = true;
        hasHit = false;
        enemiesInZone.Clear();
        Debug.Log("🟢 Fenêtre de dégâts ouverte");

        // ⛔ Supprime ou commente ça :
        // Invoke(nameof(TriggerDamage), 0.1f);
    }


    public void DisableDamageWindow()
    {
        canDealDamage = false;
        enemiesInZone.Clear();
        Debug.Log("🔴 Fenêtre de dégâts fermée");
    }

    public void TriggerDamage()
    {
        Debug.Log("✅ TriggerDamage lancé par Animation Event !");

        if (!canDealDamage || hasHit)
        {
            Debug.Log("⛔️ Pas autorisé à infliger des dégâts.");
            return;
        }

        if (enemiesInZone.Count == 0)
        {
            Debug.Log("❌ Aucun ennemi détecté dans la zone !");
            return;
        }

        bool anyHit = false;

        var enemiesToDamage = new List<Collider2D>(enemiesInZone); // ✅ copie locale

        foreach (Collider2D target in enemiesToDamage)
        {
            if (target == null) continue;

            // On cible le bon GameObject avec le bon script sans remonter au root
            if (target.TryGetComponent<Enemy>(out var e))
            {
                e.Damage(damage);
                Debug.Log("🦴 Dégâts infligés à Enemy : " + e.name);
                anyHit = true;
            }
            else if (target.TryGetComponent<BossHealth>(out var boss))
            {
                boss.TakeDamage(damage);
                Debug.Log("💀 Dégâts infligés à Boss : " + boss.name);
                anyHit = true;
            }
            else if (target.TryGetComponent<ExplosiveEnemy>(out var ex))
            {
                ex.Damage(damage);
                Debug.Log("💣 Dégâts infligés à Slime Explosif : " + ex.name);
                anyHit = true;
            }
            else if (target.TryGetComponent<FlyingEnemy>(out var fe))
            {
                fe.Damage(damage);
                Debug.Log("🪶 Dégâts infligés à Slime Volant : " + fe.name);
                anyHit = true;
            }
            else
            {
                Debug.LogWarning("⚠️ Aucun script de dégâts trouvé sur : " + target.name);
            }
        }

        if (anyHit)
            hasHit = true;
    }


}
