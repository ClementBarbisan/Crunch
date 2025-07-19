using UnityEngine;

public class Debug_Interact : MonoBehaviour, IInteractable
{


    public void Interact()
    {
        Debug.Log("Interacted");
    }
}
