using System.Collections;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("Mouvement")]
    [SerializeField]
    private float moveSpeed = 8f;

    [SerializeField]
    private float acceleration = 10f;

    [SerializeField]
    private float deceleration = 15f;
    private float moveInput;
    private bool facingRight = true;

    [Header("Saut")]
    [SerializeField]
    private float jumpForce = 12f;

    [SerializeField]
    private float groundCheckDistance = 0.1f;

    [SerializeField]
    private LayerMask groundLayer;
    private bool isGrounded;

    [Header("Physique aérienne")]
    [SerializeField]
    private float fallMultiplier = 2.5f;
    [SerializeField]
    private float lowJumpMultiplier = 2f;

    [Header("Son")]
    public AudioClip[] MashUp;
    public AudioSource JeSaute;

    [SerializeField]
    private float airControlFactor = 0.5f;
    private Rigidbody2D rb;

    void Start()
    {
        Time.timeScale = 1f;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckGround();
        Jump();
        ApplyGravityModifiers();
        HandleMovement();
        FlipCharacter();
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
        int index = Random.Range(0, MashUp.Length);
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            JeSaute.PlayOneShot(MashUp[index]);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void ApplyGravityModifiers()
    {
        if (rb.velocity.y < 0)
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
    }

    private void HandleMovement()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        float controlFactor = isGrounded ? 1f : airControlFactor;

        if (moveInput != 0)
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, moveInput * moveSpeed, acceleration * controlFactor * Time.deltaTime), rb.velocity.y);
        else
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, deceleration * Time.deltaTime), rb.velocity.y);
    }

    private void FlipCharacter()
    {
        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector2(
            -transform.localScale.x,
            transform.localScale.y
        );
    }
}