using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractableCoffeeMachine : MonoBehaviour, IInteractable
{
    public bool Heavy => true;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private float _radiusSphere = 3f;
    [SerializeField] private LayerMask _interactableLayer = 1 << 6;
    [SerializeField] private float _boostWorkCoffee = 0.3f;
    private Collider[] colliders = new Collider[10];
    private int detectedHits;

    private bool _isThrown;

    public void Interact()
    {
        
    }

    public void OnScream()
    {
        
    }

    public void OnThrow()
    {
        _isThrown = true;
    }

    private void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
            return;
        if (_isThrown)
        {
            _isThrown = false;
            _particleSystem.transform.up = Vector3.up;
            _particleSystem.gameObject.SetActive(true);
            _particleSystem.Play();
            detectedHits = Physics.OverlapSphereNonAlloc(transform.position, _radiusSphere, colliders, _interactableLayer); 
            if (detectedHits > 0)
            {
                for (int i = 0; i < detectedHits; i++)
                {
                    if (colliders[i].TryGetComponent(out NPC npcOther))
                    {
                        npcOther.WorkStress += _boostWorkCoffee;
                    }
                }
            }
        }
    }
}
