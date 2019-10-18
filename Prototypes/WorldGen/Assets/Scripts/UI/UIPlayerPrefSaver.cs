using BaD.Modules.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaD.UI.DumpA {
    [RequireComponent(typeof(InputField))]
    public class UIPlayerPrefSaver: MonoBehaviour {

        public string prefKey = "TestThing";

        public void Start () {
            string defaultValue = string.Empty;//Why not do "" in stead of string.Empty???
            InputField input = GetComponent<InputField>();
            if (input != null) {
                if (PlayerPrefs.HasKey(prefKey)) {
                    defaultValue = PlayerPrefs.GetString(prefKey);
                    input.text = defaultValue;
                }
            }

            input.onValueChanged.AddListener(OnTextChanged);
        }

        public void OnTextChanged ( string val ) {
            //Setting things
            //Launcher.worldSeed = StringToSeedNumber(val);
            PlayerPrefs.SetString(prefKey, val);
        }
    }
}