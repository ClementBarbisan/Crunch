using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class InteractableCat : MonoBehaviour, IInteractable
{
    private NavMeshAgent _agent;
    private bool _isThrown;
    private bool _held;
    private Animator _animator;
    private Rigidbody _rb;
    [SerializeField] private float _radiusSphere = 3f;
    [SerializeField] private LayerMask _interactableLayer = 1 << 6;
    [SerializeField] private float _decreaseWorkStressCat = 0.02f;
    public bool Heavy { get; }
    private int detectedHits;
    private Collider[] colliders = new Collider[10];


    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_held || _isThrown)
        {
            return;
        }
        detectedHits = Physics.OverlapSphereNonAlloc(transform.position, _radiusSphere, colliders, _interactableLayer); 
        if (detectedHits > 0)
        {
            for (int i = 0; i < detectedHits; i++)
            {
                if (colliders[i].TryGetComponent(out NPC npcOther))
                {
                    npcOther.WorkStress -= _decreaseWorkStressCat * Time.deltaTime;
                }
            }
        }
        if (_agent.velocity.magnitude < 0.1f)
        {
            _agent.SetDestination(new Vector3(transform.position.x + Random.Range(-10, 10), 0,
                transform.position.z + Random.Range(-10, 10)));
        }
    }

    public void Interact()
    {
        _agent.enabled = false;
        _animator.speed = 0;
        _held = true;
    }

    public void OnScream()
    {
        
    }

    public void OnThrow()
    {
        _held = false;
       _isThrown = true;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
            return;
        if (_isThrown)
        {
            _isThrown = false;
            _agent.enabled = true;
            _agent.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.linearVelocity = Vector3.zero;
            _rb.isKinematic = true;
            _animator.speed = 1;
            if (_agent.velocity.magnitude < 0.1f)
            {
                _agent.SetDestination(new Vector3(transform.position.x + Random.Range(-10, 10), 0,
                    transform.position.z + Random.Range(-10, 10)));
            }
        }
    }
}
