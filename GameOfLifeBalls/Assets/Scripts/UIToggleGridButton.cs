using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggleGridButton : MonoBehaviour
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
        GameOfLifeTilemapRenderer.main.ToggleGrid();
        UpdateButton();
    }

    public void UpdateButton()
    {
        txtTitle.text = GameOfLifeTilemapRenderer.main.GridIsOn ? "Grid: <color=green>ON</color>" : "Grid: <color=red>OFF</color>";
        imgBg.color = GameOfLifeTilemapRenderer.main.GridIsOn ? isOnColor : isOffColor;
    }
}
