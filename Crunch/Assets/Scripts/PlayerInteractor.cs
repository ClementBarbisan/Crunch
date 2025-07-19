using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public float interactRange = 1f;
    
    [SerializeField] private float interactRadius = .5f;
    [SerializeField] private float screamRadius = 2f;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] private Transform carryPoint;

    private InputSystem_Actions _controls;
    private bool _handFree = true;
    private Transform _interactableToThrow;
    private PlayerController _playerController;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _controls = new InputSystem_Actions();
        _controls.Player.Attack.started += _ => TryInteract();
        _controls.Player.Scream.performed += _ => Scream();
    }

    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();

    void TryInteract()
    {
        Debug.Log("Player Interact");
        if (_handFree)
        {
            // CARRY 
            
            Vector3 origin = transform.position + Vector3.up * 1f;
            Vector3 direction = transform.forward;

            if (Physics.SphereCast(origin, interactRadius, direction, out RaycastHit hit, interactRange, interactableLayer))
            {
                Debug.Log(hit.transform.name);
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                    _handFree = false;
                    
                    InteractableToThrow(hit.transform);
                }
            }
        }
        else
        {
            // THROW
            _handFree = false;

            if (_interactableToThrow != null)
            {
                //To do 
                _interactableToThrow.SetParent(null);
            }
        }
    }

    private void InteractableToThrow(Transform obj)
    {
        _interactableToThrow = obj;
        obj.GetComponent<Collider>().enabled = false;
        obj.SetParent(carryPoint);
        obj.localPosition = Vector3.zero;
        obj.localRotation = Quaternion.identity;
        _playerController.moveSpeed -= 3f;
    }

    private void Scream()
    {
        Debug.Log("Player scream");

        if (Physics.SphereCast(transform.position, screamRadius, transform.forward, out RaycastHit hit, 0f, interactableLayer))
        {
            Debug.Log(hit.transform.name);
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.OnScream();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 1f + transform.forward * interactRange, interactRadius);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, screamRadius);
    }
}