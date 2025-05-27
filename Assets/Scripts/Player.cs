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



    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }




    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, canClimb ? vertical * speed : rb.linearVelocity.y);
        animator.SetFloat("Speed", Mathf.Abs(horizontal));
        animator.SetBool("IsGrounded", IsGrounded());

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
        sr.flipX = horizontal < 0 ? true : false;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }
    }


    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.8f, groundLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Climb"))
        {
            canClimb = true;
            print("Contact !");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Climb"))
        {
            canClimb = false;
        }
    }
}
