
using System;
using UnityEngine;

public class PropsBoost : MonoBehaviour, IInteractable
{
    [SerializeField] private ParticleSystem breakFx, boostFx;
    [SerializeField] private AudioClip clipBreak, clipBoost;
    [SerializeField] private float boostEffectValue;
    [HideInInspector]
    public bool waitColliding;
    public bool Heavy => false;
    
    public void Interact()
    {
        // Nothing
    }

    public void OnScream()
    {
        // Nothing
    }

    public void OnThrow()
    {
        waitColliding = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!waitColliding)
            return;

        if (collision.transform.CompareTag("NPC"))
        {
            Boost();
            if (collision.collider.TryGetComponent(out NPC npcOther))
                npcOther.WorkStress = boostEffectValue;
        }
        else if (!collision.transform.CompareTag("Player"))
        {
            Breaks();
        }
    }

    private void Boost()
    {
        waitColliding = false;
        AudioSource.PlayClipAtPoint(clipBoost, transform.position);
        Instantiate(boostFx, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    private void Breaks()
    {
        waitColliding = false;
        AudioSource.PlayClipAtPoint(clipBreak, transform.position);
        Instantiate(breakFx, transform.position, Quaternion.identity);
        GameManager.Instance.StatsBreaks();
        Destroy(gameObject);
    }
}
