using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaD.UI.DumpA {//A place to dump UI scripts to not clutter up the unity namespaces
    [RequireComponent(typeof(InputField))]
    public class UINickNameEntry: MonoBehaviour {

        const string playerNamePrefKey = "PlayerName";

        // Start is called before the first frame update
        void Start () {
            string defaultName = string.Empty;//Why not do "" in stead of string.Empty???
            InputField inputName = GetComponent<InputField>();
            if (inputName != null) {
                if (PlayerPrefs.HasKey(playerNamePrefKey)) {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    inputName.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
            inputName.onEndEdit.AddListener(OnTextChanged);
        }

        public void OnTextChanged ( string value ) {
            if (string.IsNullOrEmpty(value)) {
                Debug.LogError("Player name is null or empty");
                return;
            }
            PhotonNetwork.NickName = value;

            PlayerPrefs.SetString(playerNamePrefKey, value);
        }
    }
}