using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public float hoverScale = 1.1f;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private Vector3 originalScale;
    private AudioSource audioSource;

    void Start()
    {
        originalScale = transform.localScale;
        audioSource = FindObjectOfType<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * hoverScale;
        if (hoverSound && audioSource) audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound && audioSource) audioSource.PlayOneShot(clickSound);
    }
}
