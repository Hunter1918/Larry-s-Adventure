using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AttackTopView : MonoBehaviour
{

    [Header("Hitboxes")]
    public GameObject normalHitbox;
    public GameObject chargedHitbox;

    [Header("Timings & Coût")]
    public float normalAttackDelay = 0.3f;
    public float chargedAttackDelay = 0.6f;
    public float chargedHealthCost = 0.2f;

    private bool isAttacking = false;
    private PlayerHealth playerHealth;
    private SpriteRenderer sr;
    private LarryControllerTopView characterController;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        sr = GetComponent<SpriteRenderer>();
        characterController = GetComponent<LarryControllerTopView>();

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
            StartCoroutine(SlowMovementTemporarily(1f, 0.5f));
        }

        Vector2 dir = characterController.GetDirection();
        if (dir == Vector2.zero) dir = Vector2.down; 

        hitboxGO.transform.position = transform.position + (Vector3)dir.normalized;
        hitboxGO.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
        hitboxGO.SetActive(true);

        PlayerMeleeHitbox hitbox = hitboxGO.GetComponent<PlayerMeleeHitbox>();
        hitbox.ShowZone();

        sr.DOKill();
        sr.color = isCharged ? Color.red : Color.yellow;
        sr.DOColor(Color.white, 0.2f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(0.1f);

        hitbox.EnableDamageWindow();
        hitbox.TriggerDamage();
        hitbox.DisableDamageWindow();

        yield return new WaitForSeconds(delay);

        hitbox.HideZone();
        hitboxGO.SetActive(false);
        isAttacking = false;

        if (isCharged)
            playerHealth.isChargingAttack = false;
    }

    IEnumerator SlowMovementTemporarily(float duration, float speedMultiplier)
    {
        float originalSpeed = characterController.moveSpeed;
        characterController.moveSpeed *= speedMultiplier;

        yield return new WaitForSeconds(duration);

        characterController.moveSpeed = originalSpeed;
    }
}