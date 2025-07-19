using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShakeCam : MonoBehaviour
{
    [SerializeField] private float shakeAmount = 0.1f;
    [SerializeField] private float decreaseFactor = 1f;
    [SerializeField] private float shake = 0.5f;
    private Vector3 initPos;


    private void Awake()
    {
        initPos = transform.localPosition;
    }

    void Update() 
    {
        if (shake > 0) 
        {
            transform.localPosition += Random.insideUnitSphere * shakeAmount;
            shake -= Time.deltaTime * decreaseFactor;
        } 
        else
        {
            transform.localPosition = initPos;
            shake = 0f;
        }
    }

}
