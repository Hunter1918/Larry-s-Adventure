using UnityEngine;

public class BossControllerTopView : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public GameObject projectilePrefab;
    public GameObject[] slimes;
    public GameObject shockwaveEffect;

    private enum BossPhase { Phase1, Phase2, Phase3 }
    private BossPhase phase;

    void Start()
    {
        currentHealth = maxHealth;
        phase = BossPhase.Phase1;
        InvokeRepeating("PerformAction", 2f, 4f);
    }

    void Update()
    {
        UpdatePhase();
    }

    void UpdatePhase()
    {
        if (currentHealth > 70f) phase = BossPhase.Phase1;
        else if (currentHealth > 40f) phase = BossPhase.Phase2;
        else phase = BossPhase.Phase3;
    }

    void PerformAction()
    {
        switch (phase)
        {
            case BossPhase.Phase1:
                ShootProjectile();
                break;
            case BossPhase.Phase2:
                int choice = Random.Range(0, 3);
                if (choice == 0) ShootProjectile();
                else if (choice == 1) SummonSlimes();
                else ZoneAttack();
                break;
            case BossPhase.Phase3:
                choice = Random.Range(0, 3);
                if (choice == 0) Shockwave();
                else if (choice == 1) SummonSlimes();
                else ShootProjectile();
                break;
        }
    }

    void ShootProjectile()
    {
        Instantiate(projectilePrefab, transform.position, Quaternion.identity);
    }

    void SummonSlimes()
    {
        Instantiate(slimes[Random.Range(0, slimes.Length)], transform.position + Random.insideUnitSphere * 3, Quaternion.identity);
    }

    void ZoneAttack()
    {
        Debug.Log("Zone Attack télégraphiée");
    }

    void Shockwave()
    {
        Instantiate(shockwaveEffect, transform.position, Quaternion.identity);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        Debug.Log("Boss mort !");
    }
}