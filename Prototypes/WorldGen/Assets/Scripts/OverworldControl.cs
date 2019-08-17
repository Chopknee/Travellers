using BaD.Modules.Terrain;
using BaD.UI.DumpA;
using Photon.Pun;
using System;
using UnityEngine;

namespace BaD.Modules {
    public class OverworldControl: MonoBehaviourPunCallbacks, IPunObservable {

        [SerializeField]
#pragma warning disable 0649
        private MapData mapGenerationInformation;

        //[SerializeField]
        //private int MapSeed = 0;
        private int mpSd = 0;

        [SerializeField]
        private GameObject buildingsPointerPrefab;

        public static OverworldControl Instance {
            get {
                return LocalPlayerReference;
            }
        }

        private static OverworldControl LocalPlayerReference;

        public Map Map {
            get {
                return GetComponent<Map>();
            }
        }

        public int NoiseSeed {
            get {
                return Map.noiseData.seed;
            }
        }

        public bool MapReady {
            get {
                return Map.Generated;
            }
        }

        private void Awake () {
            //There should only ever be a single instance of this object on every player's game.
            LocalPlayerReference = this;
            bp = Instantiate(buildingsPointerPrefab);
            bp.SetActive(false);
        }

        public void Start () {
            if (photonView.IsMine) {
                //Only sync the variables here if we are the owner of the server.
                ulong value = 0;
                ulong.TryParse(PlayerPrefs.GetString(UISeedEntry.worldSeedPrefNameKey), out value);
                mpSd = (int)value;
                mapGenerationInformation.noiseData.seed = mpSd;
                Map.Generate(mapGenerationInformation);//Since we know dat will be set at this point.
                MapGenerated = true;//Prevents double-regeneration
            }
        }

        bool MapGenerated = false;

        public void OnPhotonSerializeView ( PhotonStream stream, PhotonMessageInfo info ) {
            if (stream.IsWriting) {//Only send the data if the instance is miine????
                stream.SendNext(mpSd);
            } else {
                //Waiting for the map generation information to be synchronized.
                if (!MapGenerated) {
                    object rcvd = stream.ReceiveNext();
                    try {
                        mpSd = (int) rcvd;
                        mapGenerationInformation.noiseData.seed = mpSd;
                        Map.Generate(mapGenerationInformation);
                        MapGenerated = true;
                    } catch (InvalidCastException) {
                        Debug.LogFormat("<Color=Red>Could not receive map seed, the stream data was in the wrong format. {0}", rcvd);
                    }
                }
            }
        }

        public bool GUIOpen {
            get {
                //Check each gui to see if it is in the open state.
                return MainControl.Instance.ShopUI.activeSelf;
            }
        }

        public GameObject BuildingPointer {
            get {
                return bp;
            }
        }
        GameObject[] structures;

        public void HideOverworld() {
            Map.terrainMeshObject.SetActive(false);
            //Hide all other objects like buildings?
            structures = GameObject.FindGameObjectsWithTag("Structure");
            foreach (GameObject go in structures) {
                go.SetActive(false);
            }
            BuildingPointer.SetActive(false);
        }

        public void ShowOverworld() {
            //The map
            Map.terrainMeshObject.SetActive(true);
            //The structures
            foreach (GameObject go in structures) {
                go.SetActive(true);
            }
            BuildingPointer.SetActive(true);
            //The light as well
        }

        private GameObject bp;
    }
}
