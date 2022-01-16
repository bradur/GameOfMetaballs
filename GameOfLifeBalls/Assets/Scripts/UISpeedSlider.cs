using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpeedSlider : MonoBehaviour
{
    [SerializeField]
    Slider slider;

    void Start() {
        UpdateSpeed();
    }
    public void UpdateSpeed() {
        GOLCellGrid.main.SetSpeed((int)(slider.maxValue - slider.value));
    }
}
