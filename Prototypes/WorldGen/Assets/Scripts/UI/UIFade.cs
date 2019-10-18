using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFade: MonoBehaviour {

    public delegate void FadeCompleted ();

    public static UIFade DoFade(float fadeTime, FadeCompleted ActionOnFadeCompletion, Color fadeColor, AnimationCurve fadeCurve = null) {
        GameObject fader = new GameObject("ScreenFade");
        RectTransform rt = fader.AddComponent<RectTransform>();
        Canvas c = fader.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 100;
        CanvasScaler cs = fader.AddComponent<CanvasScaler>();
        GraphicRaycaster gr = fader.AddComponent<GraphicRaycaster>();
        CanvasRenderer cr = fader.AddComponent<CanvasRenderer>();
        Image i = fader.AddComponent<Image>();
        i.color = new Color(0, 0, 0, 0);
        UIFade uif = fader.AddComponent<UIFade>();
        uif.onFadeCompleted = ActionOnFadeCompletion;
        uif.fadeTime = fadeTime;
        uif.fadeCurve = fadeCurve;
        uif.fadeColor = fadeColor;
        return uif;
    }

    public FadeCompleted onFadeCompleted;
    public float fadeTime;
    public AnimationCurve fadeCurve;
    public Color fadeColor;
    Color blank;

    private SmoothCount transition;

    Image img;

    void Start () {
        transition = new SmoothCount(fadeCurve, 0, 1, fadeTime);
        transition.OnFinish += OnTransitionFinish;

        img = GetComponent<Image>();

        blank = fadeColor;
        blank.a = 0;

        transition.Start();
    }
    
    void Update () {
        transition.DriveForward(Time.deltaTime);
        img.color = Color.Lerp(blank, fadeColor, transition.lastValue);
    }

    public void Reverse() {
        transition.Reverse();
    }

    void OnTransitionFinish() {
        onFadeCompleted?.Invoke();
    }
}
