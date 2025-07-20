using UnityEngine;
using Random = UnityEngine.Random;

public class InteractableCoffeeMachine : MonoBehaviour, IInteractable
{
    public bool Heavy => true;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private float _radiusSphere = 3f;
    [SerializeField] private LayerMask _interactableLayer = 1 << 6;
    [SerializeField] private float _boostWorkCoffee = 0.3f;
    [SerializeField] private Transform coffeePropsPf, coffeePropsSpawnPosition;
    [SerializeField] private AudioClip clipCoffeeSpawn, coffeeMachineBreakSplash;
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
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && !_isThrown)
        {
            // Spawn coffee
            Instantiate(coffeePropsPf, coffeePropsSpawnPosition.position + new Vector3(Random.Range(-.1f, .1f), 0f, Random.Range(-.1f, .1f)), Quaternion.identity);
            AudioSource.PlayClipAtPoint(clipCoffeeSpawn, transform.position);
        }
        else if (!collision.collider.CompareTag("Player") && _isThrown)
        {
            _isThrown = false;
            Instantiate(_particleSystem, transform.position, Quaternion.identity);
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
            
            AudioSource.PlayClipAtPoint(coffeeMachineBreakSplash, transform.position);
            GameManager.Instance.StatsBreaks();
            Destroy(gameObject);
        }
    }
}
