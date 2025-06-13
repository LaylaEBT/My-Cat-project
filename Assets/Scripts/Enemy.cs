using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public int maxHealth = 1;
    protected int currentHealth; // Changed to protected to allow access in derived classes if needed

    public int damageAmount = 1; // Damage this enemy deals to the player
    public float speed = 2f;
    public Transform groundDetection;
    public float groundDetectionDistance = 1f;
    public LayerMask groundLayer;

    public float knockbackForce = 10f; // Force applied TO this enemy when hit

    protected Transform player;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator animator;

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError($"{gameObject.name}: Player not found! Make sure the player has the 'Player' tag.");
        }
        
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (player == null || !IsPlayerVisible()) // Check if player exists
        {
            animator.SetBool("IsMoving", false); // Stop moving animation if player not visible
            return;
        }

        Move();
        FlipSprite();
    }

    protected virtual void Move()
    {
        if (player == null || !IsGroundAhead()) // Check player again
        {
            animator.SetBool("IsMoving", false);
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop horizontal movement if no ground or player
            return;
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
        animator.SetBool("IsMoving", true);
    }

    protected virtual void FlipSprite()
    {
        if (player != null)
        {
            // Flip only if the enemy is moving or has a clear direction towards the player
            if (Mathf.Abs(rb.linearVelocity.x) > 0.1f || Mathf.Abs(player.position.x - transform.position.x) > 0.1f)
            {
                 sr.flipX = player.position.x < transform.position.x;
            }
        }
    }

    protected virtual bool IsGroundAhead()
    {
        // Raycast downwards from the groundDetection point
        // Consider adding a small horizontal offset to the raycast origin based on movement direction
        // to truly check "ahead" rather than just directly below.
        // For simplicity, current version checks directly below groundDetection.
        return Physics2D.Raycast(groundDetection.position, Vector2.down, groundDetectionDistance, groundLayer);
    }

    protected virtual bool IsPlayerVisible()
    {
        // This checks if the enemy's origin is within the camera viewport.
        // You might want a more sophisticated visibility check (e.g., Raycast to player, field of view).
        if (Camera.main == null) return false; // Guard against missing MainCamera
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        return viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Player playerScript = collision.collider.GetComponent<Player>();
            if (playerScript != null)
            {
                // Check if the player is in an attacking state (normal attack or roll)
                if (playerScript.isAttacking)
                {
                    Debug.Log($"Enemy '{gameObject.name}' collided with attacking Player. Player.isAttacking: {playerScript.isAttacking}. Calling TakeDamage on '{gameObject.name}'. Player attack damage: {playerScript.attackDamage}");
                    TakeDamage(playerScript.attackDamage, playerScript.transform); // Enemy takes damage from player
                    // Knockback is now handled within Enemy's TakeDamage method.
                }
                else // Player is NOT in an attack state, so player takes damage from this enemy
                {
                    Debug.Log($"Player collided with Enemy '{gameObject.name}' while not attacking. Player takes damage. Enemy damageAmount: {damageAmount}");
                    playerScript.TakeDamage(damageAmount, transform); // Player takes damage
                }
            }
            else
            {
                Debug.LogWarning($"Enemy '{gameObject.name}' collided with Player tag, but Player script not found on '{collision.collider.name}'.");
            }
        }
    }

    // Method for the enemy to take damage
    // 'attacker' is the Transform of the entity that caused the damage (e.g., the Player)
    public virtual void TakeDamage(int amount, Transform attacker) 
    {
        currentHealth -= amount;
        Debug.Log($"Enemy '{gameObject.name}' TakeDamage called. Damage: {amount}. Health before: {currentHealth + amount}, Health after: {currentHealth}. Attacker: {attacker.name}");
        // animator.SetTrigger("Hurt"); // Uncomment if you have a "Hurt" animation for the enemy

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Only apply knockback if the enemy is still alive
            ApplyKnockback(attacker); 
            Debug.Log($"Enemy '{gameObject.name}' took damage! Health left: {currentHealth}. Applied knockback.");
        }
    }

    // Method for applying knockback force to this enemy
    protected virtual void ApplyKnockback(Transform source)
    {
        if (rb == null) return; // Safety check

        Vector2 direction = (transform.position - source.position).normalized;
        if (direction == Vector2.zero) // If source is exactly on top, knock away horizontally or upwards
        {
            direction = (transform.position.x > source.position.x) ? Vector2.right : Vector2.left;
            if(direction == Vector2.zero) direction = Vector2.up; // Fallback
        }

        rb.linearVelocity = Vector2.zero; // Clear current velocity to make knockback more impactful
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        Debug.Log($"Enemy '{gameObject.name}' applied knockback from {source.name}. Direction: {direction}, Force magnitude: {knockbackForce}");

    }

    protected virtual void Die()
    {
        Debug.Log($"Enemy '{gameObject.name}' died.");
        // animator.SetTrigger("Die"); // Uncomment if you have a "Die" animation
        
        // Disable components to stop behavior
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true; // Stop physics interactions
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach(Collider2D col in colliders) col.enabled = false;
        this.enabled = false; // Disable this script

        Destroy(gameObject, 1f); // Destroy the game object after a delay (e.g., for death animation)
    }
}