using UnityEngine;
using UnityEngine.AI;

public class PlayerInteractor : MonoBehaviour
{
    public float interactRange = 1f;
    
    [SerializeField] private float interactRadius = .5f;
    [SerializeField] private float screamRadius = 2f;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] private Transform carryPoint;
    [SerializeField] private float throwForce;

    private InputSystem_Actions _controls;
    private bool _handFree = true;
    private Transform _interactableToThrow;
    private Quaternion _rotationInitThrowable;
    private PlayerController _playerController;
    private Collider _interactableDetected;
    private int _screamedDetected;
    private readonly RaycastHit[] _screamedDetectedHit = new RaycastHit[10];

    private void Awake()
    {
        _playerController = GetComponentInParent<PlayerController>();
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
                if(interactable.Heavy)
                    _playerController.speedMove = _playerController.moveSpeedSlow;
                InteractableToThrow(_interactableDetected.transform);
            }
        }
        else
        {
            // THROW
            _handFree = true;

            if (_interactableToThrow != null)
            {
                _playerController.speedMove = _playerController.moveSpeedFast;
                _interactableToThrow.SetParent(null);
                _interactableToThrow.GetComponent<Collider>().enabled = true;
                _interactableToThrow.rotation = _rotationInitThrowable;
                _interactableToThrow.GetComponent<IInteractable>().OnThrow();
                Rigidbody rb = _interactableToThrow.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
                
                if (_interactableToThrow.GetComponent<NPC>() != null)
                {
                    NPC npc = _interactableToThrow.GetComponent<NPC>();
                    npc.isHeldByPlayer = false;
                    npc.isThrown = true;
                }
                
                _interactableToThrow = null;
            }
        }
    }
    
    private void InteractableToThrow(Transform obj)
    {
        _interactableToThrow = obj;
        _rotationInitThrowable = obj.rotation;
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.GetComponent<Collider>().enabled = false;

        if (obj.GetComponent<NavMeshAgent>() != null)
        {
            if (_interactableToThrow.GetComponent<NPC>() != null)
                _interactableToThrow.GetComponent<NPC>().isHeldByPlayer = true;
            obj.GetComponent<NavMeshAgent>().isStopped = true;
        }
        
        obj.SetParent(carryPoint);
        obj.localPosition = Vector3.zero;
        obj.localRotation = Quaternion.identity;
        obj.GetComponent<Renderer>().materials[1].SetFloat("_Detected", 0f);
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
        Vector3 origin = transform.position + Vector3.up * .25f - transform.forward * 0.1f;
        Vector3 direction = transform.forward;

        if (Physics.SphereCast(origin, interactRadius, direction, out RaycastHit hit, interactRange, interactableLayer))
        {
            if (_interactableDetected != null && _interactableDetected != hit.collider)
                _interactableDetected.GetComponent<Renderer>().materials[1].SetFloat("_Detected", 0f);

            _interactableDetected = hit.collider;
            _interactableDetected.GetComponent<Renderer>().materials[1].SetFloat("_Detected", 1f);
        }
        else if (_interactableDetected)
        {
            _interactableDetected.GetComponent<Renderer>().materials[1].SetFloat("_Detected", 0f);
            _interactableDetected = null;
        }
        
        if(_interactableDetected != null)
            Debug.Log(_interactableDetected.transform.name);
        
        _screamedDetected = Physics.SphereCastNonAlloc(transform.position, interactRadius, transform.forward, _screamedDetectedHit, interactRange, interactableLayer);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * .25f + transform.forward * interactRange - transform.forward * 0.1f, interactRadius);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, screamRadius);
    }
}