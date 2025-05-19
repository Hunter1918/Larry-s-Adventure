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

        Debug.Log("⚡ TriggerDamage appelé");

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

        Collider2D target = enemiesInZone[0];
        Transform root = target.transform.root;
        bool touched = false;

        if (root.GetComponentInChildren<Enemy>() is Enemy e)
        {
            e.Damage(damage);
            Debug.Log("🦴 Dégâts infligés à Enemy : " + e.name);
            touched = true;
        }
        else if (root.GetComponentInChildren<BossHealth>() is BossHealth boss)
        {
            boss.TakeDamage(damage);
            Debug.Log("💀 Dégâts infligés à Boss : " + boss.name);
            touched = true;
        }
        else if (root.GetComponentInChildren<ExplosiveEnemy>() is ExplosiveEnemy ex)
        {
            ex.Damage(damage);
            Debug.Log("💣 Dégâts infligés à Slime Explosif : " + ex.name);
            touched = true;
        }
        else if (root.GetComponentInChildren<FlyingEnemy>() is FlyingEnemy fe)
        {
            fe.Damage(damage);
            Debug.Log("🪶 Dégâts infligés à Slime Volant : " + fe.name);
            touched = true;
        }

        if (touched)
        {
            hasHit = true;
        }
        else
        {
            Debug.LogWarning("⚠️ Aucun script de dégâts trouvé sur : " + target.name);
        }
    }
}
