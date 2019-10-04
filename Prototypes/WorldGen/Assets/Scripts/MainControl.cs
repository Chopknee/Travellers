using BaD.Chopknee.Utilities;
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
        public PlayerData LocalPlayerData { get; private set; }
        public GameObject MapControlObjectInstance { get; private set; }

        [SerializeField]
#pragma warning disable 0649
        private GameObject MapControlPrefab;
        [SerializeField]
#pragma warning disable 0649
        private GameObject OverworldPlayerPrefab;


        public GameObject DungeonPlayerPrefab;

        void Awake () {
            //Warning messages about missing objects.
            if (shopUI == null) {
                Debug.Log("<color=blue><a>Missing</a></color> Shop GUI reference.", this);
            }

            if (actionConfirmationUI == null) {
                Debug.Log("<color=blue><a>Missing</a></color> Action Confirmation reference.", this);
            }

            if (playerInventoryUI == null) {
                Debug.Log("<color=blue><a>Missing</a></color> Player Inventory reference.", this);
            }

            if (hudUI == null) {
                Debug.Log("<color=blue><a>Missing</a></color> HUD reference.", this);
            }

            if (MapControlPrefab == null) {
                Debug.Log("<color=blue><a>Missing</a></color> map prefab reference.");
            }

            if (OverworldPlayerPrefab == null) {
                Debug.Log("<color=blue><a>Missing</a></color> player prefab reference.");
            }
        }

        public void Start () {
            if (Instance == null) {
                Instance = this;
                if (PhotonNetwork.IsMasterClient) {
                    Debug.Log("<color=green>Generating</color> map for the master client.");
                    //Only the master should spawn an instance of the map. (All other players will spawn this automatically thanks to the network.)
                    MapControlObjectInstance = NetworkInstantiation.Instance.Instantiate(MapControlPrefab, false);
                    MapControlObjectInstance.GetComponent<OverworldControl>().OnFinishedGenerating += OnMapGenerationFinished;
                    MapControlObjectInstance.GetComponent<OverworldControl>().SetupMaster();
                    needsSetup = false;
                    
                }
            }
        }

        bool needsSetup = true;
        public void Update () {
            //Waiting for the overworld control to be spawned, and for the seed to be fulfilled
            if (needsSetup && !PhotonNetwork.IsMasterClient) {
                if (OverworldControl.Instance != null) {
                    if (OverworldControl.Instance.SeedReceived) {
                        Debug.Log("<color=green>Generating</color> map for regular client.");
                        OverworldControl.Instance.OnFinishedGenerating += OnMapGenerationFinished;
                        OverworldControl.Instance.SetupClient();
                        needsSetup = false;
                        
                    }
                }
            }
        }

        public void OnMapGenerationFinished() {
            //This is when we know the map has been generated!!!
            Debug.Log("<color=green>Generating</color> map has finished.");
            CollectorsCaravanStart();

        }

        public void CollectorsCaravanStart() {

            LocalPlayerObjectInstance = NetworkInstantiation.Instance.Instantiate(OverworldPlayerPrefab, true);
            LocalPlayerObjectInstance.transform.position += Vector3.up * 10;
            LocalPlayerData = new PlayerData(LocalPlayerObjectInstance);
            LocalPlayerObjectInstance.SetActive(false);//We don't need it any more.

            if (Camera.main.GetComponent<CameraMovement>() != null) {
                Camera.main.GetComponent<CameraMovement>().currentTarget = LocalPlayerObjectInstance.transform;
            }

            //At this point, we want to spawn the player in the first spawned instance of the collector's caravan.
            GameObject cc = GameObject.Find("Collectors Caravan(Clone)");
            if (cc != null) {
                Debug.Log("<color=green>Spawning</color> at collector's caravan.");
                cc.GetComponent<InstanceActivation>().DoInteraction();//Force the instance to spawn.
            } else {
                Debug.Log("<color=green>Could</color> not find an instance of the collector's caravan to start at.");
            }
        }

        public List<DungeonManager> areaStack = new List<DungeonManager>();

        public void EnterArea(DungeonManager area) {
            if (areaStack.Count == 0) {//0 means we aren't in an area
                //Hides all overworld stuff
                LocalPlayerObjectInstance.SetActive(false);
                OverworldControl.Instance.HideOverworld();
                LocalPlayerObjectInstance.SetActive(false);

            } else {
                //Just hide the previously generated instance
                //Assuming the previously generated area is still around.
                areaStack[areaStack.Count - 1].HideArea();
            }

            areaStack.Add(area);
        }

        public void ExitArea(DungeonManager area) {
            Debug.Log("Area stack as it is now; " + areaStack.Count);

            int ind = areaStack.IndexOf(area);
            areaStack.RemoveRange(ind, areaStack.Count - ind);
            Debug.Log("Area stack after moving; " + areaStack.Count);
            if (areaStack.Count == 0) {//Move back to the overworld.
                Debug.Log("Reentering the overworld!!");
                LocalPlayerObjectInstance.SetActive(true);
                OverworldControl.Instance.ShowOverworld();

                CameraMovement cf = Camera.main.GetComponent<CameraMovement>();
                cf.currentTarget = LocalPlayerObjectInstance.transform;
                cf.smoothness = 5;
                cf.pan = 50;
                cf.turnSpeed = 2;
                cf.offset = 5;
                cf.verticalOffset = 5;
                cf.zoomSensitivity = 1f;
                cf.distanceToPlayer = 2;
                cf.horizontalDistanceToPlayer = 3;
                cf.mouseSensitivity = 1.5f;

                LocalPlayerObjectInstance.SetActive(true);
            }
        }

        public int GetStackSeed() {//Generates a lovely seed based on the current stack.
            int seed = 0;
            foreach (DungeonManager dm in areaStack) {
                seed += dm.dungeonSeed;//Choptilities.Vector3ToID(dm.transform.position);
            }
            return seed;
        }
    }
}