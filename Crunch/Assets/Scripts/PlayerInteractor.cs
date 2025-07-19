using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public float interactRange = 1f;
    
    [SerializeField] private float interactRadius = .5f;
    [SerializeField] LayerMask interactableLayer;
    
    private InputSystem_Actions _controls;
    private bool _handFree = true;

    private void Awake()
    {
        _controls = new InputSystem_Actions();
        _controls.Player.Attack.started += _ => TryInteract();
    }
    
    void TryInteract()
    {
        if (!_handFree)
        {
            Vector3 origin = transform.position + Vector3.up * 1f;
            Vector3 direction = transform.forward;

            if (Physics.SphereCast(origin, interactRadius, direction, out RaycastHit hit, interactRange, interactableLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                    _handFree = false;
                }
            }
        }
        else
        {
            
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + Vector3.up * 1f + transform.forward * interactRange, interactRadius);
    }
}