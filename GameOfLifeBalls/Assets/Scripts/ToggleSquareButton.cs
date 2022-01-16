using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSquareButton : MonoBehaviour
{
    [SerializeField]
    private Color isOnColor;
    [SerializeField]
    private Color isOffColor;
    [SerializeField]
    private Text txtTitle;
    [SerializeField]
    private Image imgBg;

    private void Start()
    {
        UpdateButton();
    }

    public void Toggle()
    {
        GOLCellGrid.main.ToggleSquares();
        UpdateButton();
    }

    public void UpdateButton()
    {
        txtTitle.text = GOLCellGrid.main.SquaresAreVisible ? "Hide squares" : "Show squares";
        imgBg.color = GOLCellGrid.main.SquaresAreVisible ? isOnColor : isOffColor;
    }
}
