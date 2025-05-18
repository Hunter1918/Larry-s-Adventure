using UnityEngine;
using DG.Tweening;

public class PlayerMeleeHitbox : MonoBehaviour
{
    private Collider2D enemyInZone;
    private bool canDealDamage = false;

    private SpriteRenderer sr;
    private Collider2D col;

    public int damage = 2;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        sr.enabled = false;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Boss")) 
        {
            enemyInZone = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((other.CompareTag("Enemy") || other.CompareTag("Boss")) && other == enemyInZone)
        {
            enemyInZone = null;
        }
    }


    public void ShowZone()
    {
        DOTween.Kill("PlayerZonePulse");
        sr.DOKill();

        gameObject.SetActive(true);
        sr.enabled = true;
        col.enabled = true;

        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
        sr.DOFade(0.5f, 0.15f).SetLoops(-1, LoopType.Yoyo).SetId("PlayerZonePulse");
    }

    public void HideZone()
    {
        DOTween.Kill("PlayerZonePulse");
        sr.DOKill();

        sr.DOFade(0f, 0.1f).OnComplete(() => {
            sr.enabled = false;
            col.enabled = false;
            gameObject.SetActive(false);
        });
    }

    public void EnableDamageWindow() => canDealDamage = true;
    public void DisableDamageWindow() => canDealDamage = false;

    public void TriggerDamage()
    {
        if (!canDealDamage)
        {
            Debug.LogWarning("❌ Tentative d'infliger des dégâts alors que le DamageWindow est fermé !");
            return;
        }

        if (enemyInZone == null || enemyInZone.gameObject == null)
        {
            Debug.Log("❌ Ennemi déjà détruit !");
            return;
        }

        // Ensuite les GetComponent sécurisés
        Enemy e = enemyInZone.GetComponent<Enemy>();
        if (e != null)
        {
            e.Damage(damage);
            Debug.Log("Ennemi touché par attaque !");
            return;
        }

        BossHealth boss = enemyInZone.GetComponent<BossHealth>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Debug.Log("Boss touché par attaque !");
            return;
        }

        ExplosiveEnemy ex = enemyInZone.GetComponent<ExplosiveEnemy>();
        if (ex != null)
        {
            ex.Damage(damage);
            Debug.Log("Slime explosif touché !");
            return;
        }

        FlyingEnemy fe = enemyInZone.GetComponent<FlyingEnemy>();
        if (fe != null)
        {
            fe.Damage(damage);
            Debug.Log("Slime Vollant touché !");
            return;
        }

        Debug.LogWarning("⚠️ Aucun script d'ennemi trouvé sur " + enemyInZone.name);
    }

}
