using UnityEngine;

public class ExplosiveEnemy : MonoBehaviour
{
    private ExplosiveSlime slime;

    void Awake()
    {
        slime = GetComponent<ExplosiveSlime>();
        if (slime == null)
            Debug.LogError("❌ Aucun script ExplosiveSlime trouvé sur " + gameObject.name);
    }

    public void Damage(int amount)
    {
        if (slime != null)
            slime.TakeDamage(amount);
        else
            Debug.LogWarning("⚠️ Impossible d'infliger des dégâts : slime manquant");
    }
}
