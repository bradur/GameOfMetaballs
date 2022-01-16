using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGridButton : MonoBehaviour
{
    [SerializeField]
    private Color isOnColor;
    [SerializeField]
    private Color isOffColor;
    [SerializeField]
    private Text txtTitle;
    [SerializeField]
    private Image imgBg;

    void Start()
    {
        UpdateButton();
    }

    public void Toggle()
    {
        GOLCellGrid.main.ToggleGrid();
        UpdateButton();
    }

    public void UpdateButton()
    {
        txtTitle.text = GOLCellGrid.main.GridIsOn ? "Hide grid" : "Show grid";
        imgBg.color = GOLCellGrid.main.GridIsOn ? isOnColor : isOffColor;
    }
}