using BaD.Chopknee.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDebug : MonoBehaviour {

    [SerializeField]
#pragma warning disable 0649
    private ScrollRect scrollView;
    [SerializeField]
#pragma warning disable 0649
    private Font debugFont;
    [SerializeField]
#pragma warning disable 0649
    private int fontSize;
    [SerializeField]
#pragma warning disable 0649
    private float lineSpacing;
    [SerializeField]
#pragma warning disable 0649
    private Color fontColor;
    [SerializeField]
#pragma warning disable 0649
    private Color normalColor;
    [SerializeField]
#pragma warning disable 0649
    private Color warningColor;
    [SerializeField]
#pragma warning disable 0649
    private Color errorColor;

    public int MaxLogMessages = 500;

    CanvasGroup cg;

    List<GameObject> logMessages;

    // Start is called before the first frame update
    void Start() {

        logMessages = new List<GameObject>();

        DontDestroyOnLoad(this.gameObject);
        Application.logMessageReceived += LogLogged;
        cg = GetComponent<CanvasGroup>();
        cg.interactable = false;
        cg.blocksRaycasts = false;
        cg.alpha = 0;
    }

    private void Update () {
        if (Input.GetButtonDown("Debug")) {
            if (cg.interactable) {
                cg.interactable = false;
                cg.blocksRaycasts = false;
                cg.alpha = 0;
            } else {
                cg.interactable = true;
                cg.blocksRaycasts = true;
                cg.alpha = 1;
                
            }
        }
    }

    void LogLogged(string logString, string stackTrace, LogType type) {
        string message = logString;
        Color c = normalColor;

        if (type == LogType.Exception || type == LogType.Error || type == LogType.Warning) {
            message += "\n" + stackTrace;
            c = errorColor;
            if (type == LogType.Warning) {
                c = warningColor;
            }
        }
        GameObject go = CreateDebugMessage(c, type.ToString(), message);
        go.transform.SetParent(scrollView.content);
        logMessages.Add(go);

        if (logMessages.Count > MaxLogMessages) {
            Destroy(logMessages[0]);
            logMessages.RemoveAt(0);//Remove the oldest message from the log
        }

        Invoke("thing", 0.1f);
    }

    void thing () {
        scrollView.verticalScrollbar.value = 0f;
    }

    public GameObject CreateDebugMessage ( Color typeColor, string sender, string message ) {
        GameObject go = new GameObject("Chat Message " + sender);
        go.AddComponent<CanvasRenderer>();
        go.AddComponent<RectTransform>();
        Text tx = go.AddComponent<Text>();
        tx.font = debugFont;
        tx.fontSize = fontSize;
        tx.lineSpacing = lineSpacing;
        tx.color = fontColor;
        tx.text = string.Format("<color=#{0}>{1}</Color>: {2} ", ColorUtility.ToHtmlStringRGB(typeColor), sender, message);
        ContentSizeFitter csf = go.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;//I know this is technically not valid, but eff it.
        return go;
    }

}
