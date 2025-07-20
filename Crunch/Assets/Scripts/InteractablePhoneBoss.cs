using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractablePhoneBoss : MonoBehaviour,IInteractable
{
    [SerializeField] private ParticleSystem phoneFX, breakFX;
    [SerializeField] private AudioClip clipRing, clipBreak;
    [SerializeField] private float _stressBoost = 0.3f;
    private AudioSource _sourceAudio;
    private Animator _animator;
    private bool _ring;
    private Vector3 _initPos, _initRot;

    private void Start()
    {
        _initPos = transform.position;
        _initRot = transform.eulerAngles;
        _sourceAudio = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        //Invoke(nameof(DebugRing), Random.Range(5f,10f));
    }

    private void DebugRing()
    {
        StartCoroutine(CoroutineRing());
    }

    public void LaunchCoroutineRing()
    {
        
        StartCoroutine(CoroutineRing());
    }
    
    public IEnumerator CoroutineRing()
    {
        if (_ring)
            yield break;
        _ring = true;
        while (_ring)
        {
            RingPhone();
            yield return new WaitForSeconds(4f);
        }
    }

    public void RingPhone()
    {
        _sourceAudio.clip = clipRing;
        _sourceAudio.Play();
        _animator.SetTrigger("Ring");
        phoneFX.Play();
    }
    
    public bool Heavy { get; }
    public void Interact()
    {
        _ring = false;
        _sourceAudio.Stop();
    }

    public void OnScream()
    {
        
    }

    public void OnThrow()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent(out NPC npc))
            {
                npc.WorkStress += _stressBoost;
            }
            _sourceAudio.clip = clipBreak;
            _sourceAudio.Play();
            Instantiate(breakFX, transform.position, Quaternion.identity);
            transform.position = _initPos;
            transform.eulerAngles = _initRot;
        }
    }
}
