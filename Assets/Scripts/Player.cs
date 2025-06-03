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

    private bool isRolling = false;                  
    private float rollDuration = 0.6f;               
    private float rollTimer = 0f;                    
    private int rollDirection = 0;                   //  (-1 for left, 1 for right)
    private float lastHorizontalDirection = 1f;
    private int jumpsRemaining;
    public int maxJumps = 2;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentLives = maxLives;
    }
    
    private bool CanRoll()
{
    return !isRolling && IsGrounded();
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

        if (IsGrounded() && !isRolling)
        {
            jumpsRemaining = maxJumps;
        }


        // ✅ Trigger roll: Down + Left or Right
        // ✅ Handle rolling input: Must be holding Down + Left/Right
        bool downPressed = Input.GetKey(KeyCode.DownArrow);
        bool leftPressed = Input.GetKey(KeyCode.LeftArrow);
        bool rightPressed = Input.GetKey(KeyCode.RightArrow);
        bool rollInputHeld = downPressed && (leftPressed || rightPressed);


        // Only start rolling if eligible
        if (CanRoll() && rollInputHeld)
{
    int direction = rightPressed ? 1 : -1;
    StartRolling(direction);
}

// Stop rolling if the keys are released or time ran out
if (isRolling)
{
    rollTimer -= Time.deltaTime;

    if (!rollInputHeld || rollTimer <= 0f)
    {
        StopRolling();
    }

    // Keep the animator updated every frame while rolling
    animator.SetBool("IsRolling", true);
}
else
{
    animator.SetBool("IsRolling", false);
}

   animator.SetBool("IsGrounded", IsGrounded());
   //animator.SetBool("IsMoving", Mathf.Abs(horizontal) > 0.1f); FALSE



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
         {
        if (horizontal != 0f)
        {
            lastHorizontalDirection = horizontal;
            sr.flipX = horizontal < 0f;
        }
        else
        {
            sr.flipX = lastHorizontalDirection < 0f; // Flip based on last direction
        }
    }

        animator.SetBool("IsMoving", Mathf.Abs(horizontal) > 0.1f);

    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && jumpsRemaining > 0 && !isRolling)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            jumpsRemaining--;
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        //if (context.performed && IsGrounded() && !isRolling && !isAttacking)
        if (context.performed && IsGrounded() && !isRolling)
        {
            isAttacking = true;
            attackTimer = attackDuration;
            animator.SetTrigger("Attack");
            Debug.Log("Attack triggered");
        }
    }

    private void StartRolling(int direction)
{
    if (isRolling && rollDirection == direction) return;

    isRolling = true;
    isAttacking = true;
    rollDirection = direction;
    sr.flipX = direction == -1;
    animator.SetBool("IsRolling", true);
}


    private void StopRolling()
    {
        if (!isRolling) return;

        isRolling = false;
        isAttacking = false;
        
        animator.SetBool("IsRolling", false);
        animator.SetBool("IsMoving", false); // ✅ Also immediately clear IsMoving FALSE

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
