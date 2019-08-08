using BaD.Modules.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaD.UI.DumpA {
    [RequireComponent(typeof(InputField))]
    public class UISeedEntry: MonoBehaviour {

        public const string worldSeedPrefNameKey = "PreferedSeed";

        public void Start () {
            string defaultSeed = string.Empty;//Why not do "" in stead of string.Empty???
            InputField inputSeed = GetComponent<InputField>();
            if (inputSeed != null) {
                if (PlayerPrefs.HasKey(worldSeedPrefNameKey)) {
                    defaultSeed = PlayerPrefs.GetString(worldSeedPrefNameKey);
                    inputSeed.text = defaultSeed;
                }
            }

            inputSeed.onValueChanged.AddListener(OnTextChanged);
        }

        public void OnTextChanged ( string val ) {
            //Setting things
            //Launcher.worldSeed = StringToSeedNumber(val);
            PlayerPrefs.SetString(worldSeedPrefNameKey, StringToSeedNumber(val) + "");
        }

        public ulong StringToSeedNumber ( string val ) {
            if (val == "")
                return 0;

            ulong value = 0;
            foreach (char c in val) {
                value <<= 1;
                value = value | c;//'or' the bits together
                value <<= 1;//Shif the bits to the left by 1. (this has a pretty big impact on the final number)
            }
            return value;
        }
    }
}