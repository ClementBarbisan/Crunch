using UnityEngine;
using UnityEngine.UI;

public class StressProgressBar : MonoBehaviour
{
    public NPC Npc;
    public Vector2 screenOffset;
    [SerializeField] private RectTransform rectTr;
    [SerializeField] private SlicedFilledImage fillImage;
    [SerializeField] private Gradient colorGradientStress;

    void OnGUI()
    {
        var gotransform = Npc.TransformReferenceUI;
        Vector3 adjustedScreenPos = Camera.main.WorldToScreenPoint(gotransform.position);
        adjustedScreenPos.x += screenOffset.x;
        adjustedScreenPos.y += screenOffset.y;
        rectTr.position = adjustedScreenPos;

        fillImage.color = colorGradientStress.Evaluate(Npc.WorkStress);
        fillImage.fillAmount = ExtensionMethods.Remap(Npc.WorkStress, 0, 1, 0.05f,1);
    }

}

public static class ExtensionMethods
{

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

}


