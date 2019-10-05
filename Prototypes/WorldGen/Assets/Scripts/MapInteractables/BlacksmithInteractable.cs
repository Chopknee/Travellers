using BaD.Chopknee.Utilities;
using BaD.Modules;
using BaD.Modules.Control;
using BaD.Modules.Networking;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaD.Modules.Terrain {
    public class BlacksmithInteractable: Interactable {

        public GameObject BlacksmithGUIPrefab;
        private GameObject BlacksmithGUIInstance;

        public override void DoInteraction() {
            //Runs when this thing is clicked on.
            BlacksmithGUIInstance = Instantiate(BlacksmithGUIPrefab);
            BlacksmithGUIInstance.GetComponent<UIWindow>().OnClosed += CloseBlacksmith;
        }

        private void CloseBlacksmith () {
            //Runs when the blacksmith window is closed...
        }

        public override string GetShopName () {
            return "Blacksmith";
        }
    }
}