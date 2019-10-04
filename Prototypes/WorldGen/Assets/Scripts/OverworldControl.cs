using BaD.Modules.Terrain;
using BaD.UI.DumpA;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace BaD.Modules {
    public class OverworldControl: MonoBehaviourPunCallbacks, IPunObservable {

        public delegate void FinishedGenerating ();
        public FinishedGenerating OnFinishedGenerating;

        [SerializeField]
#pragma warning disable 0649
        private MapData mapGenerationInformation;

        private int mpSd = 0;

        [SerializeField]
        private GameObject buildingsPointerPrefab;

        public static OverworldControl Instance { get; private set; }

        bool MapGenerated = false;

        public Map Map { get { return GetComponent<Map>(); } }
        public int NoiseSeed { get { return Map.noiseData.seed; } }
        public bool MapReady { get { return Map.Generated; } }

        public bool SeedReceived { get; private set; }

        public GameObject BuildingPointer { get; private set; }

        GameObject[] structures;

        private void Awake () {
            Instance = this;

            BuildingPointer = Instantiate(buildingsPointerPrefab);
            BuildingPointer.SetActive(false);
        }

        public void SetupMaster() {
            if (PhotonNetwork.IsMasterClient) {
                //Only sync the variables here if we are the owner of the server.
                ulong value = 0;
                ulong.TryParse(PlayerPrefs.GetString(UISeedEntry.worldSeedPrefNameKey), out value);
                mpSd = (int) value;
                mapGenerationInformation.noiseData.seed = mpSd;
                Map.Generate(mapGenerationInformation);//Since we know dat will be set at this point.
                MapGenerated = true;//Prevents double-regeneration
                OnFinishedGenerating?.Invoke();
            }
        }

        public void SetupClient() {
            Map.Generate(mapGenerationInformation);
            MapGenerated = true;
            OnFinishedGenerating?.Invoke();
        }

        public void OnPhotonSerializeView ( PhotonStream stream, PhotonMessageInfo info ) {
            if (stream.IsWriting) {
                stream.SendNext(mpSd);
            } else {
                //Waiting for the map generation information to be synchronized.
                if (!MapGenerated) {
                    object rcvd = stream.ReceiveNext();
                    mpSd = (int) rcvd;
                    mapGenerationInformation.noiseData.seed = mpSd;
                    SeedReceived = true;
                }
            }
        }

        public bool GUIOpen {
            get {
                //Check each gui to see if it is in the open state.
                return MainControl.Instance.ShopUI.activeSelf;
            }
        }

        public void HideOverworld() {
            GetComponent<NavMeshSurface>().enabled = false;
            Map.terrainMeshObject.SetActive(false);
            //Hide all other objects like buildings?
            structures = GameObject.FindGameObjectsWithTag("Structure");
            foreach (GameObject go in structures) {
                go.SetActive(false);
            }
            BuildingPointer.SetActive(false);
        }

        public void ShowOverworld() {
            GetComponent<NavMeshSurface>().enabled = true;
            //The map
            Map.terrainMeshObject.SetActive(true);
            //The structures
            foreach (GameObject go in structures) {
                go.SetActive(true);
            }
            BuildingPointer.SetActive(true);
            //The light as well
        }
    }
}
