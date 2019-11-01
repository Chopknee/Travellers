using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueGrabber: MonoBehaviour {

    Slider mySlider;
    Text myText;
    void Start () {
        mySlider = GetComponentInParent<Slider>();
        if (mySlider == null) {
            enabled = false;
        }
        mySlider.onValueChanged.AddListener(ValueChanged);
        myText = GetComponent<Text>();
        myText.text = "" + mySlider.value;
    }

    
    void ValueChanged(float value) {
        myText.text = "" + value;
    }
}
