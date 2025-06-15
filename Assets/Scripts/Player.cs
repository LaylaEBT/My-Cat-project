using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpingPower = 10f;
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
    public int attackDamage = 1;

    public bool isAttacking { get; private set; } = false;
    private float attackDuration = 0.5f;
    private float attackTimer = 0f;

    private bool isRolling = false;
    private float rollDuration = 0.6f;
    private float rollTimer = 0f;
    private int rollDirection = 0; 
    
    private float lastHorizontalDirection = 1f; 
    private int jumpsRemaining;
    public int maxJumps = 2;

    public float knockbackForce = 5f; 

    private bool isClimbing = false; 
    public GameObject[] lifeIcons;
    public GameObject gameOverMenu;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentLives = maxLives;
        jumpsRemaining = maxJumps;
        Debug.Log("Player Start: Initialization complete.");
    }

    private void Update()
    {
        // Handle roll timer and state
        if (isRolling)
        {
            rollTimer -= Time.deltaTime;
            if (rollTimer <= 0f)
            {
                StopRolling(); 
            }
        }
        // Handle normal attack timer and state (only if not rolling)
        else if (isAttacking && !isRolling) // Ensure this is for normal attack, not roll's attack phase
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false; // Normal attack ends
                Debug.Log("[ATTACK DEBUG] Normal attack timer ended. isAttacking set to false for normal attack.");
            }
        }

        if (IsGrounded() && !isRolling && !isClimbing) 
        {
            jumpsRemaining = maxJumps;
        }

        // --- Climbing State Logic ---
        if (canClimb && !isRolling && !isAttacking) 
        {
            if (Mathf.Abs(vertical) > 0.05f) 
            {
                if (!isClimbing) 
                {
                    isClimbing = true;
                    rb.gravityScale = 0f;       
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
                }
            }
        }
        else 
        {
            if (isClimbing) 
            {
                isClimbing = false;
                rb.gravityScale = 1f; 
            }
        }
        // --- End Climbing State Logic ---

        // Rolling input
        bool downPressed = Input.GetKey(KeyCode.DownArrow);
        bool leftPressed = Input.GetKey(KeyCode.LeftArrow);
        bool rightPressed = Input.GetKey(KeyCode.RightArrow);
        bool rollInputActive = downPressed && (leftPressed || rightPressed);

        if (CanRoll() && rollInputActive && !isClimbing) 
        {
            int direction = rightPressed ? 1 : -1;
            StartRolling(direction);
        }

        // --- Animator Updates ---
        bool grounded = IsGrounded(); 

        if (isClimbing)
        {
            animator.SetBool("IsClimbing", true);
            animator.SetBool("IsMoving", false);  
            animator.SetBool("IsGrounded", false); 
            animator.SetBool("IsRolling", false); 
        }
        else if (isRolling) 
        {
            animator.SetBool("IsClimbing", false);
            animator.SetBool("IsRolling", true);
            animator.SetBool("IsMoving", false); 
            animator.SetBool("IsGrounded", grounded); 
        }
        else // Not Climbing and Not Rolling: Handle normal movement (Idle/Run) and Grounded state
        {
            animator.SetBool("IsClimbing", false);
            animator.SetBool("IsRolling", false);
            animator.SetBool("IsGrounded", grounded);

            bool movingHorizontally = Mathf.Abs(horizontal) > 0.05f;
            bool shouldBeMoving = movingHorizontally && grounded && !isAttacking; // Ensure not attacking to allow run animation
            
            bool previousIsMoving = animator.GetBool("IsMoving");
            animator.SetBool("IsMoving", shouldBeMoving);

            /*if (Time.frameCount % 15 == 0) // Log more frequently for this specific debug
            {
                 Debug.Log($"[RUN ANIM DEBUG] H_Input:{horizontal:F2}, Grounded:{grounded}, IsAttacking(Normal):{isAttacking && !isRolling}, Calculated IsMoving:{shouldBeMoving}, Animator IsMoving Param WAS:{previousIsMoving}, IS NOW:{animator.GetBool("IsMoving")}");
            }*/
        }
        // --- End Animator Updates ---

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    private void FixedUpdate()
    {
        // Don't apply movement if in an active normal attack aniamtion that should root the player
        if (isAttacking && !isRolling && attackTimer > 0 && IsGrounded()) // Example: root player during grounded attack
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop horizontal movement
            // return; // Optional: skip other movement logic if attack roots
        }


        if (isRolling)
        {
            rb.linearVelocity = new Vector2(rollDirection * speed * 3f, rb.linearVelocity.y);
        }
        else if (isClimbing)
        {
            rb.gravityScale = 0f; 
            float climbVerticalVelocity = vertical * speed; 
            float climbHorizontalVelocity = horizontal * speed * 0.75f;
            rb.linearVelocity = new Vector2(climbHorizontalVelocity, climbVerticalVelocity);
        }
        else // Normal movement (not rolling, not climbing, and potentially not rooted by attack)
        {
            rb.gravityScale = 1f; 
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        horizontal = input.x;
        vertical = input.y; 

        if (!isRolling && !isClimbing) 
        {
            if (Mathf.Abs(horizontal) > 0.01f) 
            {
                lastHorizontalDirection = Mathf.Sign(horizontal); 
                sr.flipX = horizontal < 0f;
            }
        }
    }
    
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && !isRolling && !(isAttacking && !isRolling && attackTimer > 0)) // Cannot jump while rolling or during active normal attack
        {
            if (isClimbing) 
            {
                isClimbing = false; 
                float jumpOffWallX = lastHorizontalDirection * -1f * speed * 0.7f; 
                rb.linearVelocity = new Vector2(jumpOffWallX, 0); 
                rb.AddForce(new Vector2(0f, jumpingPower), ForceMode2D.Impulse); 
                jumpsRemaining = maxJumps - 1; 
                animator.SetTrigger("Jump"); 
                Debug.Log("Player event: Jumped from climbing state.");
            }
            else if (jumpsRemaining > 0) 
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
                rb.AddForce(new Vector2(0f, jumpingPower), ForceMode2D.Impulse);
                jumpsRemaining--;
                animator.SetTrigger("Jump"); 
            }
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        bool canAttackConditions = !isRolling && !isClimbing && (IsGrounded() || jumpsRemaining < maxJumps);
        
        if (context.performed && canAttackConditions) 
        {
            isAttacking = true; // This is for normal attacks
            attackTimer = attackDuration; // Use normal attackDuration
            animator.SetTrigger("Attack");
            Debug.Log($"[ATTACK DEBUG] Player event: Normal Attack triggered. isAttacking set to true (for normal attack). attackTimer set to {attackTimer:F2}");
        }
        else if (context.performed)
        {
            Debug.Log($"[ATTACK DEBUG] Player Normal Attack prevented: isRolling={isRolling}, isClimbing={isClimbing}, IsGrounded={IsGrounded()}, jumpsRemaining={jumpsRemaining}");
        }
    }

    private void StartRolling(int direction)
    {
        if (isRolling && rollDirection == direction) return; 

        isRolling = true;
        isAttacking = true;     // Rolling is an attack state
        rollTimer = rollDuration; 
        attackTimer = rollDuration; // Roll's "attack" phase duration
        rollDirection = direction;
        sr.flipX = direction == -1; 
        Debug.Log("[ATTACK DEBUG] Player event: Started rolling. isRolling=true, isAttacking=true (for roll). attackTimer set for roll.");
    }

    private void StopRolling()
    {
        if (!isRolling) return; 

        isRolling = false;
        // When roll stops, its specific "attack" phase ends.
        // isAttacking should be false UNLESS a normal attack was initiated just as roll ended (unlikely with current input scheme).
        isAttacking = false; 
        attackTimer = 0f; 
        Debug.Log("[ATTACK DEBUG] Player event: Stopped rolling. isRolling=false, isAttacking=false. attackTimer reset.");
    }

    private bool CanRoll()
    {
        return !isRolling && IsGrounded() && !isClimbing && !(isAttacking && !isRolling && attackTimer > 0); // Cannot roll during active normal attack
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.8f, groundLayer);
    }

    public void TakeDamage(int amount, Transform attacker)
    {
        Debug.Log($"[KNOCKBACK DEBUG] TakeDamage called. isRolling={isRolling}, isAttacking (flag state)={isAttacking}, attackTimer={attackTimer:F2}");

        // Invulnerability conditions:
        bool invulnerableDueToRoll = isRolling; 
        bool invulnerableDueToNormalAttack = isAttacking && !isRolling && attackTimer > 0;

        if (invulnerableDueToRoll)
        {
            Debug.Log($"[KNOCKBACK DEBUG] Player invulnerable due to ROLLING. No damage or knockback.");
            return;
        }
        if (invulnerableDueToNormalAttack)
        {
             Debug.Log($"[KNOCKBACK DEBUG] Player invulnerable due to NORMAL ATTACK (attackTimer: {attackTimer:F2}). No damage or knockback.");
            return;
        }

        currentLives -= amount;
        Debug.Log($"Player took {amount} damage from {attacker.name}. Lives left: {currentLives}");
        
        
        ApplyKnockback(attacker);
        UpdateUI(); 

        if (currentLives <= 0)
        {
            //animator.SetTrigger("Die");
            Debug.Log("Player event: Died.");
            Time.timeScale = 0f; // Pause the game
            gameOverMenu.SetActive(true);
        }
    }
    void UpdateUI()
    {
        
        // Update life icons
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].SetActive(i < currentLives);
        }
    }

    public void ApplyKnockback(Transform attacker)
    {

        /*if (rb == null) return; // Safety check

        Vector2 direction = (transform.position - attacker.position).normalized;
        if (direction == Vector2.zero) // If source is exactly on top, knock away horizontally or upwards
        {
            direction = (transform.position.x > attacker.position.x) ? Vector2.right : Vector2.left;
            if(direction == Vector2.zero) direction = Vector2.up; // Fallback
        }
        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse); // knockDir is already normalized in effect if directionX is -1 or 1
        Debug.Log($"[KNOCKBACK DEBUG] ApplyKnockback: Applied force knockbackForce(knockDir:, knockbackForce: {knockbackForce}). Player Velocity AFTER: {rb.linearVelocity}");*/
        Debug.Log($"[KNOCKBACK DEBUG] Player.ApplyKnockback from {attacker.name}. Player Velocity BEFORE: {rb.linearVelocity}");

        if (attacker == null)
        {
            Debug.LogError("[KNOCKBACK DEBUG] Player.ApplyKnockback: Attacker transform is null! Cannot apply knockback.");
            return;
        }
        if (rb == null)
        {
            Debug.LogError("[KNOCKBACK DEBUG] Player.ApplyKnockback: Rigidbody2D is null! Cannot apply knockback.");
            return;
        }

        // Determine the horizontal direction away from the attacker
        float horizontalDirection;
        if (Mathf.Approximately(transform.position.x, attacker.position.x))
        {
            // If attacker is directly above/below or at same X, use last faced direction to knock away
            horizontalDirection = -lastHorizontalDirection;
            if (Mathf.Approximately(horizontalDirection, 0f)) horizontalDirection = 1f; // Default to right if lastHorizontalDirection was somehow 0
        }
        else
        {
            horizontalDirection = Mathf.Sign(transform.position.x - attacker.position.x);
        }

        // Ensure horizontalDirection is definitively -1 or 1
        if (Mathf.Approximately(horizontalDirection, 0f))
        {
            horizontalDirection = 1f; // Fallback if all else fails, knock to the right
        }

        // Create a purely horizontal knockback vector
        Vector2 knockDir = new Vector2(horizontalDirection, 0f);
        // No need to normalize if horizontalDirection is already -1 or 1, as vector length will be 1.

        rb.linearVelocity = Vector2.zero; // Clear current velocity for consistent knockback
        rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);

        Debug.Log($"[KNOCKBACK DEBUG] Player.ApplyKnockback: Applied force {knockDir * knockbackForce} (knockDir: {knockDir}, knockbackForce (from Inspector): {this.knockbackForce}). Player Velocity AFTER: {rb.linearVelocity}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Climb"))
        {
            canClimb = true;
        }
        else if (collision.CompareTag("Death"))
        {
           Time.timeScale = 0f; // Pause the game
            gameOverMenu.SetActive(true); 
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