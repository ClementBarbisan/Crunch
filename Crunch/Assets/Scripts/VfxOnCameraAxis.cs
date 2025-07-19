using UnityEngine;

public class VfxOnCameraAxis : MonoBehaviour
{
    [SerializeField] private Transform refTransform;
    [Range(0, 1), SerializeField] private float refToCamDistanceRatio = 0.5f;
    private void Update()
    {
        Vector3 dir = (Camera.main.transform.position - refTransform.position);
       transform.position = refTransform.position + dir * refToCamDistanceRatio;
    }
}
