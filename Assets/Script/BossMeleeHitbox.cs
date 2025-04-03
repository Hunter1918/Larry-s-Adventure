using UnityEngine;
using DG.Tweening;

public class BossMeleeHitbox : MonoBehaviour
{
    private Collider2D playerInZone;
    private bool canDealDamage = false;

    private SpriteRenderer sr;

    public int damage = 1;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other == playerInZone)
        {
            playerInZone = null;
        }
    }

    public void ShowZone()
    {
        gameObject.SetActive(true);
        sr.enabled = true;

        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.5f).SetEase(Ease.OutElastic);
        sr.DOFade(0.5f, 0.2f).SetLoops(-1, LoopType.Yoyo).SetId("ZonePulse");
    }

    public void HideZone()
    {
        DOTween.Kill("ZonePulse");
        sr.DOFade(0f, 0.1f).OnComplete(() => gameObject.SetActive(false));
    }

    public void EnableDamageWindow() => canDealDamage = true;
    public void DisableDamageWindow() => canDealDamage = false;

    public void TriggerDamage()
    {
        if (canDealDamage && playerInZone != null)
        {
            PlayerHealth ph = playerInZone.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
                Debug.Log("Le joueur est touché !");
            }
        }
    }
}
