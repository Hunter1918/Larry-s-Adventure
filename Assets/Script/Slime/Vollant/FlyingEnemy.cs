using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    private FlyingDiverSlime slime;

    void Awake()
    {
        slime = GetComponent<FlyingDiverSlime>();
    }

    public void Damage(int amount)
    {
        if (slime != null)
        {
            slime.TakeDamage(amount);
        }
    }
}
