
using UnityEngine;

public class PropsBoost : MonoBehaviour, IInteractable
{
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
        
    }
}
