using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMetaballRadiusSlider : MonoBehaviour
{


    [SerializeField]
    Slider slider;
    void Start() {
        UpdateRadius();
    }
    public void UpdateRadius() {
        MetaballRenderer.main.SetMetaballRadius(slider.value);
    }

}
