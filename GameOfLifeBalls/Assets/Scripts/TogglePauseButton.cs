using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TogglePauseButton : MonoBehaviour
{
    [SerializeField]
    private Color isOnColor;
    [SerializeField]
    private Color isOffColor;
    [SerializeField]
    private Text txtTitle;
    [SerializeField]
    private Image imgBg;

    public static TogglePauseButton main;
    private void Awake() {
        main = this;
    }

    void Start()
    {
        UpdateButton();
    }

    public void Toggle()
    {
        GOLCellGrid.main.ToggleDrawingPause();
        UpdateButton();
    }

    public void UpdateButton()
    {
        txtTitle.text = GOLCellGrid.main.DrawingPausesIsOn ? "Pause on click: <color=green>ON</color>" : "Pause on click: <color=red>OFF</color>";
        imgBg.color = GOLCellGrid.main.DrawingPausesIsOn ? isOnColor : isOffColor;
    }
}
