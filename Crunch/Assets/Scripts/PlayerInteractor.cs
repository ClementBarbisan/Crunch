using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float throwForceHeavy;

    private InputSystem_Actions _controls;
    private bool _handFree = true;
    private Transform _interactableToThrow;
    private Quaternion _rotationInitThrowable;
    private PlayerController _playerController;
    private Collider _interactableDetected;
    private int _screamedDetected;
    private readonly RaycastHit[] _screamedDetectedHit = new RaycastHit[10];

    [Header("Actions Vfx")]
    [SerializeField] private ParticleSystem[] textScreamVfxs;
    [SerializeField] private ParticleSystem waveScreamVfx;
    [SerializeField] private Renderer _faceRenderer;

    [Header("Animations")]
    [SerializeField] Animator animator;
    private string _isHoldingObjectParamName = "isHoldingObject";
    private string _isThrowingObjectParamName = "isThrowingObject";

    [SerializeField] private AudioSource sourceAudioScream, sourceAudioInteract;
    [SerializeField] private AudioClip[] clipsScreams;
    [SerializeField] private AudioClip clipGrab, clipThrow;
    
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

                if (animator != null)
                {
                    animator.SetBool(_isHoldingObjectParamName, true);
                }
                
                sourceAudioInteract.clip = clipGrab;
                sourceAudioInteract.Play();
            }
        }
        else
        {
            // THROW
            //Throw ();
            BeginThrowAnim();
        }
    }

    private void BeginThrowAnim()
    {
        animator.SetBool(_isThrowingObjectParamName, true);
    }
    public void Throw()
    {
        _handFree = true;

        if (_interactableToThrow != null)
        {
            _playerController.speedMove = _playerController.moveSpeedFast;
            _interactableToThrow.SetParent(null);
            _interactableToThrow.GetComponent<Collider>().enabled = true;
            _interactableToThrow.rotation = _rotationInitThrowable;
            IInteractable interactableScript = _interactableToThrow.GetComponent<IInteractable>();
            interactableScript.OnThrow();
            Rigidbody rb = _interactableToThrow.GetComponent<Rigidbody>();
            rb.isKinematic = false;

            //float adjustedThrowForce = interactableScript.Heavy ? throwForceHeavy : throwForce;
            rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);

            if (_interactableToThrow.GetComponent<NPC>() != null)
            {
                NPC npc = _interactableToThrow.GetComponent<NPC>();

                npc.OnThrow();
            }

            _interactableToThrow = null;
            
            if (animator != null)
            {
                animator.SetBool(_isThrowingObjectParamName, false);
                animator.SetBool(_isHoldingObjectParamName, false);
            }

            sourceAudioInteract.clip = clipThrow;
            sourceAudioInteract.Play();
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
            NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
            if(agent.enabled)
                agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }
        
        obj.SetParent(carryPoint);
        obj.localPosition = Vector3.zero;
        obj.localRotation = Quaternion.identity;
        SetOutline(obj, 0);
    }

    private void Scream()
    {
        if (!sourceAudioScream.isPlaying)
        {
            sourceAudioScream.clip = clipsScreams[Random.Range(0, clipsScreams.Length)];
            sourceAudioScream.Play();
        }
        
        if (textScreamVfxs.Length > 0)
        {
            textScreamVfxs[Random.Range(0, textScreamVfxs.Length)].Play();
        }
        waveScreamVfx.Play();
        StartCoroutine(SwitchFaceRenderer());

        if (_screamedDetected == 0)
            return;

        for (int j = 0; j < _screamedDetected; j++)
        {
            IInteractable interactable = _screamedDetectedHit[j].transform.GetComponent<IInteractable>();
            if (interactable != null)
                interactable.OnScream();
        }
    }

    private IEnumerator SwitchFaceRenderer()
    {
        _faceRenderer.material.SetFloat("_faceNumber", 1);
        yield return new WaitForSeconds(0.5f);
        _faceRenderer.material.SetFloat("_faceNumber", 0);
    }

    private void Update()
    {
        Vector3 origin = transform.position + Vector3.up * .25f - transform.forward * 0.2f;
        Vector3 direction = transform.forward;

        if (Physics.SphereCast(origin, interactRadius, direction, out RaycastHit hit, interactRange, interactableLayer))
        {
            if (_interactableDetected != null && _interactableDetected != hit.collider) 
                SetOutline(_interactableDetected.transform,0);

            _interactableDetected = hit.collider;
            SetOutline(_interactableDetected.transform,1);
        }
        else if (_interactableDetected)
        {
            SetOutline(_interactableDetected.transform,0);
            _interactableDetected = null;
        }
        
        _screamedDetected = Physics.SphereCastNonAlloc(transform.position, screamRadius, transform.forward, _screamedDetectedHit, 0f, interactableLayer);
    }

    private void SetOutline(Transform t, int value)
    {
        if(t.GetComponent<Renderer>() != null)
            t.GetComponent<Renderer>().materials[1].SetFloat("_Detected", value);
        else if (t.GetComponentInChildren<Renderer>() != null)
            t.GetComponentInChildren<Renderer>().materials[1].SetFloat("_Detected", value);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * .25f + transform.forward * interactRange - transform.forward * 0.1f, interactRadius);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, screamRadius);
    }
}