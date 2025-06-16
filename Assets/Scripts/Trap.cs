using UnityEngine;

/// <summary>
/// This script defines the behavior for a trap object.
/// When a GameObject with the "Player" tag enters its trigger,
/// it will deal damage to that player.
/// </summary>
public class Trap : MonoBehaviour
{

    public int damageAmount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
 
        if (collision.CompareTag("Player"))
        {

            Player player = collision.GetComponent<Player>();

            if (player != null)
            {

                player.TakeDamage(damageAmount, transform);
            }
        }
    }
}
