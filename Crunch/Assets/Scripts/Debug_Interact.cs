using UnityEngine;

public class Debug_Interact : MonoBehaviour, IInteractable
{
    public bool Heavy { get; }

    public void Interact()
    {
        Debug.Log("Interacted");
    }

    public void OnScream()
    {
        Debug.Log("Screamed at");
    }
}
