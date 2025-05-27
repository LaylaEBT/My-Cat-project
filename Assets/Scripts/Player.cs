using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpingPower;
    public LayerMask groundLayer;
    public Transform groundCheck;
    float horizontal;
    float vertical; 
    SpriteRenderer sr;
    Animator animator;
    Rigidbody2D rb; 
    public bool canClimb = false;

    public int maxLives = 5;
    private int currentLives;

    public bool isAttacking { get; private set; } = false;
    private float attackDuration = 0.5f;
    private float attackTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentLives = maxLives;
    }

    private void Update()
    {
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        animator.SetBool("IsGrounded", IsGrounded());
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, canClimb ? vertical * speed : rb.linearVelocity.y);
        animator.SetFloat("Speed", Mathf.Abs(horizontal));
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        horizontal = input.x;
        vertical = input.y;
        sr.flipX = horizontal < 0;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            isAttacking = true;
            attackTimer = attackDuration;
            animator.SetTrigger("Attack");
        }
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.8f, groundLayer);
    }

    public void TakeDamage(int amount)
    {
        if (isAttacking) return; // Optional: invulnerable while attacking

        currentLives -= amount;
        animator.SetTrigger("Hurt");
        ApplyKnockback();

        if (currentLives <= 0)
        {
            // Handle death
            animator.SetTrigger("Die");
            Debug.Log("Player died");
        }
        else
        {
            Debug.Log($"Player took damage! Lives left: {currentLives}");
        }
    }
    private void ApplyKnockback()
    {
    Vector2 knockDir = (transform.position - Camera.main.transform.position).normalized;
    rb.AddForce(knockDir * 5f, ForceMode2D.Impulse); // Knocked away from center
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Climb"))
        {
            canClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Climb"))
        {
            canClimb = false;
        }
    }
}
