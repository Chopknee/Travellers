using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBox : MonoBehaviour {
    [SerializeField]
#pragma warning disable 0649
    private ScrollRect chatLog;
    [SerializeField]
#pragma warning disable 0649
    private InputField input;
    [SerializeField]
#pragma warning disable 0649
    private Font chatFont;
    [SerializeField]
#pragma warning disable 0649
    private int fontSize;
    [SerializeField]
#pragma warning disable 0649
    private int lineSpacing;
    [SerializeField]
#pragma warning disable 0649
    private Color baseChatColor;

    Chatterbox chatterbox;
    
    bool opened = false;

    // Start is called before the first frame update
    void Start() {
        input.onEndEdit.AddListener(OnDoneMessage);
        chatterbox = GetComponent<Chatterbox>();
        chatterbox.OnTextMessageReceived += OnTextMessageReceived;
    }

    void OnTextMessageReceived(string sender, string message) {
        GameObject mess = CreateChatMessage(sender, message);
        mess.transform.SetParent(chatLog.content);
        Invoke("thing", 0.1f);
        
    }

    void thing() {
        chatLog.verticalScrollbar.value = 0f;
    }

    void OnDoneMessage(string value) {
        if (value != "") {
            chatterbox.SendTextMessage(value);
            OnTextMessageReceived(PhotonNetwork.NickName, value);
            input.text = "";
            input.ActivateInputField();
            input.Select();
        }
    }
    
    private void Update () {
        if (Input.GetButtonDown("OpenChat") && !opened) {
            SetVisible(true);
            input.ActivateInputField();
            input.Select();
        } else if (opened && Input.GetButtonDown("Escape") || !input.isFocused && Input.GetButtonDown("OpenChat")) {
            SetVisible(false);
        }
    }


    public void SetVisible(bool value) {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.alpha = ( value ) ? 1 : 0;
        cg.blocksRaycasts = value;
        cg.interactable = value;
        opened = value;
    }

    public GameObject CreateChatMessage(string sender, string message) {
        GameObject go = new GameObject("Chat Message " + sender);
        go.AddComponent<CanvasRenderer>();
        go.AddComponent<RectTransform>();
        Text tx = go.AddComponent<Text>();
        tx.font = chatFont;
        tx.fontSize = fontSize;
        tx.lineSpacing = lineSpacing;
        tx.color = baseChatColor;
        tx.text = string.Format("<color=#FBFBFB>{0}</Color>: {1} ", sender, message);
        ContentSizeFitter csf = go.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;//I know this is technically not valid, but eff it.
        return go;
    }
}
