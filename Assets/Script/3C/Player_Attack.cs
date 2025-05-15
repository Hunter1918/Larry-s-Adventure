using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Player_Attack : MonoBehaviour
{
    public GameObject normalHitbox;
    public GameObject chargedHitbox;

    public float normalAttackDelay = 0.3f;
    public float chargedAttackDelay = 0.6f;
    public float chargedHealthCost = 0.2f;

    private bool isAttacking = false;
    private PlayerHealth playerHealth;
    private SpriteRenderer sr;
    private CharacterController characterController;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        sr = GetComponent<SpriteRenderer>();
        characterController = GetComponent<CharacterController>();

        normalHitbox.SetActive(false);
        chargedHitbox.SetActive(false);
    }

    void Update()
    {
        if (isAttacking) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(DoAttack(normalHitbox, normalAttackDelay, false));
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            float cost = Mathf.Ceil(playerHealth.maxHealth * chargedHealthCost);
            if (playerHealth.GetCurrentHealth() > cost)
            {
                playerHealth.TakeDamage((int)cost);
                StartCoroutine(DoAttack(chargedHitbox, chargedAttackDelay, true));
            }
            else
            {
                Debug.Log("Pas assez de vie pour attaque chargée !");
            }
        }
    }

    IEnumerator DoAttack(GameObject hitboxGO, float delay, bool isCharged)
    {
        isAttacking = true;

        if (isCharged)
        {
            playerHealth.isChargingAttack = true;
        }

        PlayerMeleeHitbox hitbox = hitboxGO.GetComponent<PlayerMeleeHitbox>();
        hitbox.ShowZone();

        sr.DOKill();
        Color flashColor = isCharged ? Color.red : Color.yellow;
        sr.color = flashColor;
        sr.DOColor(Color.white, 0.2f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(0.1f);

        hitbox.EnableDamageWindow();
        hitbox.TriggerDamage();
        hitbox.DisableDamageWindow();

        yield return new WaitForSeconds(delay);

        hitbox.HideZone();
        isAttacking = false;

        if (isCharged)
            playerHealth.isChargingAttack = false;
    }
}