using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    [SerializeField]
    private Color isOnColor;
    [SerializeField]
    private Color isOffColor;
    [SerializeField]
    private Text txtTitle;
    [SerializeField]
    private Image imgBg;


    void Start() {
        Toggle();
    }

    public void Toggle() {

        txtTitle.text = GOLCellGrid.main.GameIsOn ? "Pause game" : "Start game";
        imgBg.color = GOLCellGrid.main.GameIsOn ? isOnColor : isOffColor;
    }
}
