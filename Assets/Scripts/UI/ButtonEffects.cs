using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEffects : MonoBehaviour
{
    public AudioSource hoverSound;

    public void OnHoverEnter()
    {
        transform.localScale = Vector3.one * 1.1f; // Make it bigger
        if (hoverSound != null)
            hoverSound.Play(); // Play hover sound
    }

    public void OnHoverExit()
    {
        transform.localScale = Vector3.one; // Reset size
    }
}