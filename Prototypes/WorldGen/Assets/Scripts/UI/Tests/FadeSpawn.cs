using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeSpawn: MonoBehaviour {
    // Start is called before the first frame update

    public AnimationCurve ac;
    public Color fadeColor;
    public float time = 10;

    UIFade uif;

    void Start () {
        uif = UIFade.DoFade(time, OnFadeComplete, fadeColor, ac);
    }

    public void OnFadeComplete() {
        Debug.Log("Fading has finished!");
        uif.Reverse();
    }
}
