using BaD.Modules.Networking;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaD.Modules {
    public class MainControl: MonoBehaviourPunCallbacks {

        public static MainControl Instance { get; private set; }

        public const byte SpawnMapControl = 100;
        public const byte SpawnPlayerObject = 101;

        public HashSet<int> usedNetworkIds;

        public GameObject ShopGUI;
        public GameObject ActionConfirmationGUI;

        [SerializeField]
#pragma warning disable 0649
        private GameObject MapControlPrefab;
        [SerializeField]
#pragma warning disable 0649
        public GameObject OverworldPlayerPrefab;

        void Awake () {
            if (ShopGUI == null) {
                Debug.Log("<Color=Blue><a>Missing</a><Color> Shop GUI prefab reference.", this);
            }

            if (ActionConfirmationGUI == null) {
                Debug.Log("<Color=Blue><a>Missing</a><Color> Action Confirmation prefab reference.", this);
            }

            if (MapControlPrefab == null) {
                Debug.Log("<Color=Blue><a>Missing</a><Color> map prefab reference.", this);
            }

            if (OverworldPlayerPrefab == null) {
                Debug.Log("<Color=Blue><a>Missing</a><Color> player prefab reference.", this);
            }
            usedNetworkIds = new HashSet<int>();
        }

        public void Start () {
            if (Instance == null) {
                Instance = this;//Prevent dual running of the setup code???!!?!?!?
                if (PhotonNetwork.IsMasterClient) {
                    //Debug.Log("I am marked as master, and am spawning the map and player object.");
                    //Only the master should spawn an instance of the map.
                    NetworkInstantiation.Instance.Instantiate(MapControlPrefab, false, ReceiverGroup.Others, EventCaching.AddToRoomCache);
                    //SpawnNetworkedMaster(MapControlPrefab, ReceiverGroup.Others, EventCaching.AddToRoomCache, SpawnMapControl);//Skipping over using the jankey workaround type code photon is built with??
                }
                NetworkInstantiation.Instance.Instantiate(OverworldPlayerPrefab, true, ReceiverGroup.Others, EventCaching.AddToRoomCache);
                //SpawnNetworkedAsOwner(OverworldPlayerPrefab, ReceiverGroup.Others, EventCaching.AddToRoomCache, SpawnPlayerObject);
            }
        }
    }
}