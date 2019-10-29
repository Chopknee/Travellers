using BaD.Modules;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPlayerInfo : MonoBehaviour {

    public enum ShownData { Name, Gold };

    public ShownData visibleData;

    TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update () {
        if (MainControl.LocalPlayerData == null) {
            return;
        }
        string txt = "";
        switch (visibleData) {
            case ShownData.Gold:
                txt = MainControl.LocalPlayerData.gold + "g";
                break;
            case ShownData.Name:
                txt = PhotonNetwork.NickName;
                break;
        }
        text.text = txt; 
    }
}
