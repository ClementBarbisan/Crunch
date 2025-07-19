using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float lerp;
    private Vector3 _offset;
    void Start()
    {
        _offset = transform.position - target.position;
    }
    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + _offset, Time.deltaTime * lerp);
    }
}
