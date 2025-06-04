using UnityEngine;


public abstract class Enemy : MonoBehaviour
{
    public int maxHealth = 1; 
    private int currentHealth;

    public int damageAmount = 1;
    public float speed = 2f;
    public Transform groundDetection;
    public float groundDetectionDistance = 1f;
    public LayerMask groundLayer;

    public float knockbackForce = 5f; 

    protected Transform player;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator animator;

    protected virtual void Start()
    {
        currentHealth = maxHealth; 

        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (!IsPlayerVisible()) return;

        Move();
        FlipSprite();
    }

    protected virtual void Move()
    {
        if (!IsGroundAhead()) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
        animator.SetBool("IsMoving", true);
    }

    protected virtual void FlipSprite()
    {
        if (player != null)
        {
            sr.flipX = player.position.x < transform.position.x;
        }
    }

    protected virtual bool IsGroundAhead()
    {
        return Physics2D.Raycast(groundDetection.position, Vector2.down, groundDetectionDistance, groundLayer);
    }

    protected virtual bool IsPlayerVisible()
    {
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
                if (playerScript.isAttacking)
                {
                    TakeDamage(1, playerScript.transform); 
                    ApplyKnockback(playerScript.transform); 
                }
                else
                {
                    playerScript.TakeDamage(damageAmount);
                    playerScript.ApplyKnockback(transform);
                }
            }
        }
    }

    
    protected virtual void TakeDamage(int amount, Transform attacker)
    {
        currentHealth -= amount;
        //animator.SetTrigger("Hurt");

        ApplyKnockback(attacker);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void ApplyKnockback(Transform source)
    {
        Vector2 direction = (transform.position - source.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * knockbackForce, rb.linearVelocity.y);
    }

    protected virtual void Die()
    {
        //animator.SetTrigger("Die");
        rb.linearVelocity = Vector2.zero;
        this.enabled = false;
        Destroy(gameObject, 1f); 
    }
}
