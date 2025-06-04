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

    private bool isRolling  = false;
                    
    private float rollDuration = 0.6f;               
    private float rollTimer = 0f;                    
    private int rollDirection = 0;                   //  (-1 for left, 1 for right)
    private float lastHorizontalDirection = 1f;
    private int jumpsRemaining;
    public int maxJumps = 2;

    private bool isClimbing = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentLives = maxLives;
    }
    


    private void Update()
{
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

    // Rolling input
    bool downPressed = Input.GetKey(KeyCode.DownArrow);
    bool leftPressed = Input.GetKey(KeyCode.LeftArrow);
    bool rightPressed = Input.GetKey(KeyCode.RightArrow);
    bool rollInputHeld = downPressed && (leftPressed || rightPressed);

    if (CanRoll() && rollInputHeld)
    {
        int direction = rightPressed ? 1 : -1;
        StartRolling(direction);
    }

        if (isRolling)
        {
            rollTimer -= Time.deltaTime;

            if (!rollInputHeld || rollTimer <= 0f)
            {
                StopRolling();
            }

            animator.SetBool("IsRolling", true);
            isAttacking = true;
        }
        else
        {
            animator.SetBool("IsRolling", false);
            isAttacking = false;
        }

    // ✅ Handle climbing input priority
    bool isClimbInput = Mathf.Abs(vertical) > 0.1f;
    isClimbing = canClimb && isClimbInput && !isRolling && !isAttacking;
    animator.SetBool("IsClimbing", isClimbing);

    // ❗Only update IsGrounded if NOT climbing
    if (!isClimbing)
    {
        animator.SetBool("IsGrounded", IsGrounded());
    }

        // ❗Only update IsMoving if NOT climbing
        if (!isClimbing)
        {
            animator.SetBool("IsMoving", Mathf.Abs(horizontal) > 0.1f);
            animator.SetBool("IsGrounded", IsGrounded()); //A CONFIRMER
    }

    if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
}

    private void FixedUpdate()
    {
        if (isRolling)
        {
            rb.linearVelocity = new Vector2(rollDirection * speed * 3f, rb.linearVelocity.y);  // ✅ Fast roll movement
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
    isAttacking = true;//Useless, WHY?
    rollDirection = direction;
    sr.flipX = direction == -1;
    animator.SetBool("IsRolling", true);
}


    private void StopRolling()
    {
        if (!isRolling) return;

        isRolling = false;
        isAttacking = false; //Useless, WHY?

        animator.SetBool("IsRolling", false);
        animator.SetBool("IsMoving", false); // Stop rolling without movement

    }

        private bool CanRoll() 
    {
    return !isRolling && IsGrounded();
    }


    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.8f, groundLayer);
    }

    public void TakeDamage(int amount)
    {
        if (isAttacking) return;

        currentLives -= amount;
        //animator.SetTrigger("Hurt");
        //ApplyKnockback(); TO PUT BACK LATER

        if (currentLives <= 0)
        {
            //animator.SetTrigger("Die");
            Debug.Log("Player died");
        }
        else
        {
            Debug.Log($"Player took damage! Lives left: {currentLives}");
        }
    }

    public void ApplyKnockback(Transform attacker)
    {
        Vector2 knockDir = (transform.position - attacker.position).normalized;
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
