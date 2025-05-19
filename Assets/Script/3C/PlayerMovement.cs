using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Mouvement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;
    [SerializeField] private float airControlFactor = 0.5f;

    [Header("Saut")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Physique aérienne")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Son")]
    public AudioClip[] MashUp;
    public AudioSource JeSaute;

    private Rigidbody2D rb;
    private Animator animator;
    private float moveInput;
    private bool facingRight = true;
    private bool isGrounded;

    public bool FacingRight => facingRight;
    public bool IsGrounded => isGrounded;

    void Start()
    {
        Time.timeScale = 1f;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGround();

        Jump();
        ApplyGravityModifiers();
        HandleMovement();
        FlipCharacter();

        // Mise à jour de l’Animator
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isJumping", rb.velocity.y > 0.1f && !isGrounded);
        animator.SetBool("isFalling", rb.velocity.y < -0.1f && !isGrounded);
    }

    private void CheckGround()
    {
        Vector2 leftRayOrigin = new Vector2(transform.position.x - 0.57f, transform.position.y - 0.6f);
        Vector2 rightRayOrigin = new Vector2(transform.position.x + 0.57f, transform.position.y - 0.6f);

        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, groundCheckDistance, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, groundCheckDistance, groundLayer);

        Debug.DrawRay(leftRayOrigin, Vector2.down * groundCheckDistance, Color.red);
        Debug.DrawRay(rightRayOrigin, Vector2.down * groundCheckDistance, Color.blue);

        isGrounded = leftHit.collider != null || rightHit.collider != null;
    }

    private void Jump()
    {
        // Saut seulement si au sol et pas d’attaque en cours
        Player_Attack attackScript = GetComponent<Player_Attack>();
        if (attackScript != null && attackScript.IsAttacking()) return;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            int index = Random.Range(0, MashUp.Length);
            JeSaute.PlayOneShot(MashUp[index]);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void ApplyGravityModifiers()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        Player_Attack attackScript = GetComponent<Player_Attack>();
        if (attackScript != null && attackScript.IsAttacking()) return;

        moveInput = Input.GetAxisRaw("Horizontal");
        float controlFactor = isGrounded ? 1f : airControlFactor;

        if (moveInput != 0)
        {
            float targetSpeed = moveInput * moveSpeed;
            rb.velocity = new Vector2(
                Mathf.Lerp(rb.velocity.x, targetSpeed, acceleration * controlFactor * Time.deltaTime),
                rb.velocity.y
            );
        }
        else
        {
            rb.velocity = new Vector2(
                Mathf.Lerp(rb.velocity.x, 0, deceleration * Time.deltaTime),
                rb.velocity.y
            );
        }
    }

    private void FlipCharacter()
    {
        if (moveInput > 0 && !facingRight || moveInput < 0 && facingRight)
        {
            facingRight = !facingRight;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
    }
}
