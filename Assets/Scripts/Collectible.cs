using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Find the GameManager and increment the score
            GameManager.instance.AddCollectible();

            // Destroy the collectible object
            Destroy(gameObject);
        }
    }
}