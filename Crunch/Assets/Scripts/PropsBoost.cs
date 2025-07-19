
using UnityEngine;

public class PropsBoost : MonoBehaviour, IInteractable
{
    [SerializeField] private ParticleSystem breakFx;
    [SerializeField] private AudioClip clipBreak;
    [HideInInspector]
    public bool waitColliding;
    public bool Heavy => false;
    
    public void Interact()
    {
        // Nothing
    }

    public void OnScream()
    {
        // Nothing
    }

    public void OnThrow()
    {
        waitColliding = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!waitColliding)
            return;

        if (!collision.transform.CompareTag("Player"))
        {
            Breaks();
        }
    }

    private void Breaks()
    {
        waitColliding = true;
        AudioSource.PlayClipAtPoint(clipBreak, transform.position);
        Instantiate(breakFx, transform.position, Quaternion.identity);
        GetComponent<MeshRenderer>().enabled = false;
        Destroy(gameObject);
    }
}
