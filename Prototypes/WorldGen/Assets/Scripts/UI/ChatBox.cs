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
        chatterbox.SendTextMessage(value);
        OnTextMessageReceived(PhotonNetwork.NickName, value);
        input.text = "";
    }
}
