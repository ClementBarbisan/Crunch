
using UnityEngine;

public class PropsBoost : MonoBehaviour, IInteractable
{
    [SerializeField] private ParticleSystem breakFx;
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
        breakFx.Play();
        GetComponent<MeshRenderer>().enabled = false;
        Invoke(nameof(DestroyGameObject), 1f);
    }

    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
