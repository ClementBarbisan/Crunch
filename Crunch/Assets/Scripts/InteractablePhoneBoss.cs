using System;
using UnityEngine;

public class InteractablePhoneBoss : MonoBehaviour,IInteractable
{
    [SerializeField] private ParticleSystem phoneFX;
    private AudioSource _sourceAudio;
    private Animator _animator;

    private void Start()
    {
        _sourceAudio = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        InvokeRepeating(nameof(RingPhone), 5f, 4f);
    }

    public void RingPhone()
    {
        _sourceAudio.Play();
        _animator.SetTrigger("Ring");
    }
    
    public bool Heavy { get; }
    public void Interact()
    {
        _sourceAudio.Stop();
    }

    public void OnScream()
    {
        
    }

    public void OnThrow()
    {
        
    }
}
