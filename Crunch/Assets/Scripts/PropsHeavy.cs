using NUnit.Framework.Constraints;
using UnityEngine;

public class PropsHeavy : MonoBehaviour, IInteractable
{
    public bool Heavy => true;
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
        
    }
}
