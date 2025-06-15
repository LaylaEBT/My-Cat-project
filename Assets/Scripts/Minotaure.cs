using UnityEngine;

public class Minotaur : Enemy
{
    [Header("Minotaur Specifics")]
    public float attackRange = 2.0f;

    private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");

    protected override void Start()
    {
        base.Start();
        maxHealth = 15;
        currentHealth = maxHealth;
        damageAmount = 3;
    }

    // [UPDATED] The Move method is changed to allow movement during the attack animation.
    protected override void Move()
    {
        // First, perform the basic checks from the base class.
        if (player == null || !IsGroundAhead())
        {
            // If the player doesn't exist or there's no ground, stop all animations and movement.
            animator.SetBool("IsMoving", false);
            animator.SetBool(IsAttackingHash, false); 
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // --- Movement Logic ---
        // This part now runs regardless of whether the Minotaur is attacking or not.
        // This ensures the Minotaur constantly moves towards the player.
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);

        // --- Animation Logic ---
        // This part determines which animation to play based on distance.
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Player is in attack range. Play the "Attack" animation.
            // We set "IsMoving" to false to ensure the "Walk" animation doesn't override the "Attack" one.
            animator.SetBool(IsAttackingHash, true);
            animator.SetBool("IsMoving", false);
        }
        else
        {
            // Player is out of range. Play the "Walk" animation.
            animator.SetBool(IsAttackingHash, false);
            animator.SetBool("IsMoving", true);
        }
    }

    protected override void FlipSprite()
    {
        if (player != null)
        {
            sr.flipX = player.position.x > transform.position.x;
        }
    }
}