using BaD.Modules.Networking;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chatterbox : Messaging {

    public delegate void TextMessageReceived ( string userNick, string messageText );
    public TextMessageReceived OnTextMessageReceived;

    //The specific data for this will be formatted as { message id, user nick, message }
    public void SendTextMessage(string text) {
        SendNetMessage(new object[] { PhotonNetwork.NickName, text });
    }

    public override void MessageReceived ( object[] messageData ) {
        //The first index of message data is message meta information including message ID and view ID.
        MessageMeta messageMeta = (MessageMeta) messageData[0];
        OnTextMessageReceived?.Invoke((string) messageData[1], (string) messageData[2]);
    }
}
