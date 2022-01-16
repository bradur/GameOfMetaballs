using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISliderValueDisplay : MonoBehaviour
{
    [SerializeField]
    Slider slider;

    [SerializeField]
    private float multiplier = 1f;
    [SerializeField]
    private float addition = 0f;
    [SerializeField]
    private string unit = "";

    [SerializeField]
    private bool round = true;
    [SerializeField]
    private int roundToPlaces = 2;

    [SerializeField]
    private bool isInt;

    [SerializeField]
    private Text txtValue;

    void Start()
    {
        UpdateValue();
    }
    public void UpdateValue()
    {
        float newValue = (slider.value + addition) * multiplier;
        if (isInt)
        {
            txtValue.text = $"{(int)newValue}{unit}";
        }
        else if (round)
        {
            txtValue.text = $"{System.Math.Round(newValue, roundToPlaces)}{unit}";
        }
        else
        {
            txtValue.text = $"{newValue}{unit}";
        }
    }
}
