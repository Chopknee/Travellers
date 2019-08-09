using BaD.Modules.Networking;
using BaD.UI.DumpA;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaD.Modules {
    public class MainControl: MonoBehaviourPunCallbacks {

        public static MainControl Instance { get; private set; }

        [SerializeField]
#pragma warning disable 0649
        private GameObject shopUI;
        public GameObject ShopUI { get { return shopUI; } }

        [SerializeField]
#pragma warning disable 0649
        private GameObject actionConfirmationUI;
        public GameObject ActionConfirmationUI { get { return actionConfirmationUI; } }

        [SerializeField]
#pragma warning disable 0649
        private GameObject playerInventoryUI;
        public GameObject PlayerInventoryUI { get { return playerInventoryUI; } }
        public UIPlayerInventory PlayerInventoryInstance {
            get {
                return PlayerInventoryUI.GetComponent<UIPlayerInventory>();
            }
        }

        [SerializeField]
#pragma warning disable 0649
        private GameObject hudUI;
        public GameObject HudUI { get { return hudUI; } }

        public GameObject LocalPlayerObjectInstance { get; private set; }

        [SerializeField]
#pragma warning disable 0649
        private GameObject MapControlPrefab;
        [SerializeField]
#pragma warning disable 0649
        private GameObject OverworldPlayerPrefab;

        void Awake () {
            //Warning messages about missing objects.
            if (shopUI == null) {
                Debug.Log("<Color=Blue><a>Missing</a><Color> Shop GUI reference.", this);
            }

            if (actionConfirmationUI == null) {
                Debug.Log("<Color=Blue><a>Missing</a><Color> Action Confirmation reference.", this);
            }

            if (playerInventoryUI == null) {
                Debug.Log("<Color=Blue><a>Missing</a><Color> Player Inventory reference.", this);
            }

            if (hudUI == null) {
                Debug.Log("<Color=Blue><a>Missing</a><Color> HUD reference.", this);
            }

            if (MapControlPrefab == null) {
                Debug.Log("<Color=Blue><a>Missing</a><Color> map prefab reference.", this);
            }

            if (OverworldPlayerPrefab == null) {
                Debug.Log("<Color=Blue><a>Missing</a><Color> player prefab reference.", this);
            }
        }

        public void Start () {
            if (Instance == null) {
                Instance = this;
                if (PhotonNetwork.IsMasterClient) {
                    //Only the master should spawn an instance of the map. (All other players will spawn this automatically thanks to the network.)
                    NetworkInstantiation.Instance.Instantiate(MapControlPrefab, false, ReceiverGroup.Others, EventCaching.AddToRoomCache);
                }
                LocalPlayerObjectInstance = NetworkInstantiation.Instance.Instantiate(OverworldPlayerPrefab, true, ReceiverGroup.Others, EventCaching.AddToRoomCache);
            }
        }
    }
}