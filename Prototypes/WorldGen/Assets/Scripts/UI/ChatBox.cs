using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBox : MonoBehaviour {
    [SerializeField]
#pragma warning disable 0649
    private Text TextArea;
    [SerializeField]
#pragma warning disable 0649
    private InputField input;
    Chatterbox chatterbox;

    bool opened = false;

    // Start is called before the first frame update
    void Start() {
        input.onEndEdit.AddListener(OnDoneMessage);
        chatterbox = GetComponent<Chatterbox>();
        chatterbox.OnTextMessageReceived += OnTextMessageReceived;
    }

    void OnTextMessageReceived(string sender, string message) {
        TextArea.text += string.Format("{0}] {1}\n", sender, message);
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
        opened = value;
    }
}
