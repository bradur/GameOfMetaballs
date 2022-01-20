using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoadRLEButton : MonoBehaviour
{
    [SerializeField]
    InputField inputField;
    public void LoadRLE() {
        GameOfLife.main.LoadPattern(inputField.text);
    }
}
