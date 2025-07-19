using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SliderController : MonoBehaviour
{
    public TMP_Text valueText;
    public void OnSliderChanged(float value)
    {
        valueText.text = value.ToString();
    }
}
