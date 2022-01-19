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
        UpdateButton();
    }

    public void Toggle()
    {
        GameOfLife.main.ToggleGame();
        UpdateButton();
    }

    public void UpdateButton() {

        txtTitle.text = GameOfLife.main.GameIsOn ? "Pause game" : "Start game";
        imgBg.color = GameOfLife.main.GameIsOn ? isOnColor : isOffColor;
    }

}
