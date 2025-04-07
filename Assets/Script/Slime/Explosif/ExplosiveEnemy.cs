using UnityEngine;

public class ExplosiveEnemy : MonoBehaviour
{
    private ExplosiveSlime slime;

    void Awake()
    {
        slime = GetComponent<ExplosiveSlime>();
    }

    public void Damage(int amount)
    {
        Debug.Log("ExplosiveEnemy: Damage reçu : " + amount);
        if (slime != null)
        {
            slime.TakeDamage(amount);
        }
        else
        {
            Debug.LogWarning("slime (ExplosiveSlime) est null !");
        }
    }
}
