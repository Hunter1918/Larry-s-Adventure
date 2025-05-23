﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Détection du joueur")]
    public LayerMask playerLayer;
    public float visionRange = 5f;
    public Vector2 raycastOffset = new Vector2(0, 0.5f);
    public bool faceRight = true;

    [Header("Patrouille")]
    public Transform platformStart;
    public Transform platformEnd;
    [SerializeField] private Vector3 destination;
    public float moveSpeed = 2f;

    [Header("Préparation Attaque")]
    public float preAttackDuration = 0.5f;

    [Header("Attaque")]
    public float attackCooldown = 2f;
    private float lastAttackTime;
    public int damageToPlayer = 1;

    [Header("Vie du Slime")]
    [SerializeField] private int health = 100;

    [Header("Effet de recul")]
    public float knockbackForce = 5f;

    [Header("Debug")]
    public bool showRaycast = true;

    private Transform targetPlayer;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;

    private bool canMove = true;
    private bool isAttacking = false;
    private bool isPlayerInRange = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        destination = platformEnd.position;
    }


    void Update()
    {
        isPlayerInRange = CanSeePlayer();

        if (canMove && !isAttacking)
        {
            Patrol();
        }

        if (isPlayerInRange && Time.time > lastAttackTime + attackCooldown && !isAttacking)
        {
            StartCoroutine(AttackSequence());
        }
    }

    void Patrol()
    {
        animator.SetBool("isWalking", true);

        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, destination) < 0.1f)
        {
            destination = (destination == platformStart.position) ? platformEnd.position : platformStart.position;
            Flip();
        }
    }

    void Flip()
    {
        faceRight = !faceRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    bool CanSeePlayer()
    {
        Vector2 direction = faceRight ? Vector2.right : Vector2.left;
        Vector2 origin = (Vector2)transform.position + raycastOffset;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, visionRange, playerLayer);

        if (showRaycast)
            Debug.DrawRay(origin, direction * visionRange, Color.red);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            targetPlayer = hit.transform;
            return true;
        }

        return false;
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;
        canMove = false;

        animator.SetBool("isWalking", false); // passe de Marche à Idle
        yield return new WaitForSeconds(preAttackDuration); // animation Idle = pré attaque

        animator.SetTrigger("TriggerAttack"); // enchaîne vers Attack
        yield return new WaitForSeconds(1f); // laisse le temps à l'animation de se jouer

        lastAttackTime = Time.time;
        canMove = true;
        isAttacking = false;
    }

    // Appelé par un Animation Event dans l’animation Attack
    public void DealDamage()
    {
        if (targetPlayer != null && isPlayerInRange)
        {
            PlayerHealth playerHealth = targetPlayer.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log($"{gameObject.name} inflige des dégâts via animation !");
                playerHealth.TakeDamage(damageToPlayer);
            }
        }
    }

    public void Damage(int amount)
    {
        health -= amount;
        Debug.Log($"{gameObject.name} a pris {amount} dégâts. Vie restante : {health}");

        Vector2 knockDirection = ((Vector2)transform.position - PlayerPosition()).normalized;
        rb.AddForce(knockDirection * knockbackForce, ForceMode2D.Impulse);

        StartCoroutine(FlashSprite());
        StartCoroutine(DisableMovement(0.5f));

        if (health <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} est mort !");
        Destroy(gameObject);
    }

    IEnumerator FlashSprite()
    {
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
        }

        spriteRenderer.color = Color.white;
    }

    IEnumerator DisableMovement(float duration)
    {
        canMove = false;
        yield return new WaitForSeconds(duration);
        canMove = true;
    }

    Vector2 PlayerPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            return player.transform.position;
        return transform.position;
    }
}
