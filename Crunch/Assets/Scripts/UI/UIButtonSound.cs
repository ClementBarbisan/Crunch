using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip hoverClip;
    [SerializeField] AudioClip clickClip;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverClip != null)
            audioSource.PlayOneShot(hoverClip);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickClip != null)
            audioSource.PlayOneShot(clickClip);
    }
}