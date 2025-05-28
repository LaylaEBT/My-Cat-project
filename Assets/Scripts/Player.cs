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

    private bool isRolling = false;                  // ✅ NEW
    private float rollDuration = 0.6f;               // ✅ NEW
    private float rollTimer = 0f;                    // ✅ NEW
    private int rollDirection = 0;                   // ✅ NEW (-1 for left, 1 for right)

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentLives = maxLives;
    }

    private void Update()
    {
        // ✅ Handle rolling state
        if (isRolling)
        {
            rollTimer -= Time.deltaTime;
            if (rollTimer <= 0f)
            {
                isRolling = false;
                isAttacking = false;
            }
        }
        else if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
            }
        }

        // ✅ Trigger roll: Down + Left or Right
        // ✅ Handle rolling input: Must be holding Down + Left/Right
    if (IsGrounded() && Input.GetKey(KeyCode.DownArrow))
{
    if (Input.GetKey(KeyCode.RightArrow))
    {
        StartRolling(1);
    }
    else if (Input.GetKey(KeyCode.LeftArrow))
    {
        StartRolling(-1);
    }
    else
    {
        StopRolling(); // No left/right input while down is held
    }
}
else
{
    StopRolling(); // Not holding down key
}


        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        animator.SetBool("IsGrounded", IsGrounded());
    }

    private void FixedUpdate()
    {
        if (isRolling)
        {
            rb.linearVelocity = new Vector2(rollDirection * speed * 2f, rb.linearVelocity.y);  // ✅ Fast roll movement
        }
        else
        {
            rb.linearVelocity = new Vector2(horizontal * speed, canClimb ? vertical * speed : rb.linearVelocity.y);
            animator.SetFloat("Speed", Mathf.Abs(horizontal));
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        horizontal = input.x;
        vertical = input.y;

        if (!isRolling) // ✅ Prevent flip override during roll
            sr.flipX = horizontal < 0;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded() && !isRolling)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded() && !isRolling)
        {
            isAttacking = true;
            attackTimer = attackDuration;
            animator.SetTrigger("Attack");
        }
    }

    private void StartRolling(int direction)
{
    if (isRolling && rollDirection == direction) return; // Already rolling that way

    isRolling = true;
    isAttacking = true;
    rollDirection = direction;
    sr.flipX = direction == -1;
    animator.SetBool("IsRolling", true); // ✅ Use Bool instead of Trigger
}

private void StopRolling()
{
    if (!isRolling) return;

    isRolling = false;
    isAttacking = false;
    animator.SetBool("IsRolling", false);
}


    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.8f, groundLayer);
    }

    public void TakeDamage(int amount)
    {
        if (isAttacking) return;

        currentLives -= amount;
        animator.SetTrigger("Hurt");
        ApplyKnockback();

        if (currentLives <= 0)
        {
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
        rb.AddForce(knockDir * 5f, ForceMode2D.Impulse);
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
