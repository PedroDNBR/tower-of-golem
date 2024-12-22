using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    Slider staminaSlider;

    public void ConvertStaminaTimeToSliderValue(float current, float max)
    {
        float sliderValue = Mathf.InverseLerp(0f, max, current);
        SetStaminaSliderValue(sliderValue);
    }

    public void SetStaminaSliderValue(float value)
    {
        staminaSlider.value = value;
    }
}
