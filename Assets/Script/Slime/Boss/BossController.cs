using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BossController : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public GameObject meleeHitbox;
    public GameObject projectilePrefab;
    public AudioClip chargeClip;
    public AudioClip impactClip;
    public SpriteRenderer spriteRenderer;

    private AudioSource audioSource;

    [Header("Combat")]
    public float meleeRange = 3f;
    public float meleeCooldown = 2f;
    public float projectileSpeed = 8f;

    private float meleeTimer = 0f;
    private bool isAttacking = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isAttacking) return;

        meleeTimer -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= meleeRange)
        {
            if (meleeTimer <= 0f)
            {
                StartCoroutine(MeleeAttack());
                meleeTimer = meleeCooldown;
            }
        }
    }

    IEnumerator MeleeAttack()
    {
        isAttacking = true;
        spriteRenderer.color = Color.yellow;

        BossMeleeHitbox hitbox = meleeHitbox.GetComponent<BossMeleeHitbox>();
        hitbox.ShowZone();
        if (chargeClip != null)
            audioSource.PlayOneShot(chargeClip);

        Debug.Log("Préparation de l'attaque");

        yield return new WaitForSeconds(1f);

        spriteRenderer.DOColor(Color.white, 0.05f).SetLoops(2, LoopType.Yoyo);
        if (impactClip != null)
            audioSource.PlayOneShot(impactClip);

        Debug.Log("Frappe !");

        hitbox.EnableDamageWindow();
        hitbox.TriggerDamage();
        hitbox.DisableDamageWindow();

        yield return new WaitForSeconds(0.3f);

        hitbox.HideZone();

        yield return new WaitForSeconds(2f);

        spriteRenderer.color = Color.white;
        isAttacking = false;
    }
}
