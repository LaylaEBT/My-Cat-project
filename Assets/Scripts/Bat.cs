using UnityEngine;

public class Bat : Enemy
{
    [Header("Bat Specific")]
    public float verticalSpeed = 2f; // Speed of the up and down movement
    public float verticalAmplitude = 1f; // How high and low the bat will fly

    private Vector3 startPosition;

    // We override the Start method from the Enemy class
    protected override void Start()
    {
        base.Start(); // This calls the Start method of the base Enemy class

        // Set bat-specific values
        maxHealth = 3;
        damageAmount = 2;
        currentHealth = maxHealth; // Ensure current health is updated

        // Store the initial position for the sine wave movement
        startPosition = transform.position;

        // Flying creatures don't need gravity
        if (rb != null)
        {
            rb.gravityScale = 0;
        }
    }

    // We override the main Update loop to remove calls to the "IsMoving" animator parameter
    protected override void Update()
    {
        // We still want the bat to stop if the player isn't visible or doesn't exist.
        if (player == null || !IsPlayerVisible())
        {
            // Simply stop movement. No animator calls are needed.
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // If the player is visible, proceed with movement and flipping the sprite.
        Move();
        FlipSprite();
    }

    // We override the Move method to implement custom flying behavior without animator logic
    protected override void Move()
    {
        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Horizontal movement towards the player
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float horizontalMovement = directionToPlayer.x * speed;

        // Vertical sine wave movement
        float verticalMovement = Mathf.Sin(Time.time * verticalSpeed) * verticalAmplitude;
        
        // Combine horizontal and vertical movement
        rb.linearVelocity = new Vector2(horizontalMovement, verticalMovement);

        // All lines related to animator.SetBool("IsMoving", ...) have been removed.
    }

    // We can disable the ground check for a flying enemy
    protected override bool IsGroundAhead()
    {
        // A flying creature doesn't need to check for the ground to move.
        return true;
    }
}