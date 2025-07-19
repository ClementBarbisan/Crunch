using UnityEngine;

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
    private Collider _interactableDetected;
    private int _screamedDetected;
    private RaycastHit[] _screamedDetectedHit = new RaycastHit[10];

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
        if (_handFree)
        {
            if (_interactableDetected == null)
                return;
            
            // CARRY 
            IInteractable interactable = _interactableDetected.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
                _handFree = false;
                    
                InteractableToThrow(_interactableDetected.transform);
            }
        }
        else
        {
            // THROW
            _handFree = true;

            if (_interactableToThrow != null)
            {
                _interactableToThrow.SetParent(null);
                _interactableToThrow.GetComponent<Collider>().enabled = true;
                _interactableToThrow = null;
                
                //To do Throw  
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
        if (_screamedDetected == 0)
            return;

        for (int j = 0; j < _screamedDetected; j++)
        {
            IInteractable interactable = _screamedDetectedHit[j].transform.GetComponent<IInteractable>();
            if (interactable != null)
                interactable.OnScream();
        }
    }

    private void Update()
    {
        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 direction = transform.forward;

        if (Physics.SphereCast(origin, interactRadius, direction, out RaycastHit hit, interactRange, interactableLayer))
            _interactableDetected = hit.collider;
        else
            _interactableDetected = null;
        
        _screamedDetected = Physics.SphereCastNonAlloc(transform.position, interactRadius, transform.forward, _screamedDetectedHit, interactRange, interactableLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 1f + transform.forward * interactRange, interactRadius);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, screamRadius);
    }
}