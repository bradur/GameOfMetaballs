using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResetAreaButton : MonoBehaviour
{
    public static UIResetAreaButton main;
    private void Awake()
    {
        main = this;
    }
    [SerializeField]
    private Slider slider;

    void Start()
    {
        ResetArea();
    }

    public void ResetArea()
    {
        GameOfLife.main.DrawArea((int)slider.value);
    }

    public void SetSliderValue(int value)
    {
        slider.value = value;
    }

}
