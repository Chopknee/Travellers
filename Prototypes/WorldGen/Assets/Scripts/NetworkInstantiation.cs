using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaD.Modules.Networking {
    public class NetworkInstantiation: MonoBehaviourPunCallbacks, IOnEventCallback {

        //Much neater than the way I have previously done this.
        public static NetworkInstantiation Instance { get; private set; }

        //Some codes for instance synchronization between clients
        private readonly static byte SpawnObjectCode = 5;
        //private readonly static byte DestroyObjectCode = 6;

        [SerializeField]
#pragma warning disable 0649
        private GameObject[] spawnablesPool;//Time to go swimming!
        private Dictionary<int, NetworkInstance> instancePool;

        void Awake() {
            if (Instance == null) {//Instance will only ever be equal to the first occurance of this object.
                Instance = this;
            }
            instancePool = new Dictionary<int, NetworkInstance>();
        }

        public void OnEvent ( EventData photonEvent ) {
            try {
                object[] data = (object[]) photonEvent.CustomData;
                if (photonEvent.Code == SpawnObjectCode) {
                    //Data '0' should always be the network id for object spawns
                    //Data '1' is the instance index to spawn
                    //Spawn an object
                    
                    int viewID = (int) data[0];
                    int prefabPoolIndex = (int) data[1];
                    //Debug.Log("Spawning object with index of " + prefabPoolIndex + " which is prefab " + spawnablesPool[prefabPoolIndex].name);
                    if (data.Length == 2) {
                        ReceivedSpawnNetworked(spawnablesPool[prefabPoolIndex], viewID);
                    } else if (data.Length == 3) {
                        //We have position!
                        ReceivedSpawnNetworked(spawnablesPool[prefabPoolIndex], viewID, (Vector3)data[2], Quaternion.identity);
                    } else if (data.Length > 3) {
                        ReceivedSpawnNetworked(spawnablesPool[prefabPoolIndex], viewID, (Vector3) data[2], (Quaternion) data[3]);
                    }
                }
            } catch (ArgumentException ex) {
                Debug.Log("<Color=Yellow>There was an argument exception whilst trying to decode an RPC event:</Color>\n" + ex.ToString(), this);
            } catch (Exception ex) {
                Debug.Log("<Color=Yellow>There was an unexpected exception whilst trying to decode an RPC event:</Color>\n" + ex.ToString(), this);
            }

        }

        public void ReceivedSpawnNetworked ( GameObject prefab, int viewId ) {
            ReceivedSpawnNetworked(prefab, viewId, Vector3.zero, Quaternion.identity);
        }

        public void ReceivedSpawnNetworked(GameObject prefab, int viewId, Vector3 position, Quaternion rotation) {
            if (!instancePool.ContainsKey(viewId)) {
                GameObject go = Instantiate(prefab);
                go.transform.position = position;
                go.transform.rotation = rotation;
                PhotonView view = go.GetComponent<PhotonView>();
                view.ViewID = viewId;
                instancePool.Add(view.ViewID, new NetworkInstance(view.ViewID, go));
            } else {
                //Debug.Log("A call to spawn an already instantiated networked object was ignored.");
            }
        }

        //Events for registering this to listen for RPCs
        public override void OnEnable () {
            base.OnEnable();
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable () {
            base.OnDisable();
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public GameObject Instantiate(GameObject prefab, bool asOwner, ReceiverGroup receivers, EventCaching eventCacheOption) {
            int ind = -1;
            for (int i = 0; i < spawnablesPool.Length; i++) {
                if (prefab == spawnablesPool[i]) ind = i;
            }
            if (ind == -1) {
                Debug.LogFormat("<Color=Red>Failed to network instantiate game object {0}, it was not in the spawn pool.</Color>", prefab.name);
                return null;
            }
            //Debug.Log("Requested instantiation was found in the prefabs pool at " + ind + " " + spawnablesPool[ind]);
            return Instantiate(ind, asOwner, receivers, eventCacheOption);
        }

        public GameObject Instantiate ( int prefabIndex, bool asOwner, ReceiverGroup receivers, EventCaching eventCacheOption ) {
            if (prefabIndex >= spawnablesPool.Length) {
                Debug.LogFormat("<Color=Red>Failed to network instantiate object, prefab index {0} does not exist.</Color>", prefabIndex);
                return null;
            }

            GameObject go = Instantiate(spawnablesPool[prefabIndex]);
            PhotonView view = go.GetComponent<PhotonView>();

            if (view != null) { 
                //Basically; before the ||, if as owner is false, the second statement is not executed and skips to after the ||
                if ( asOwner && PhotonNetwork.AllocateViewID(view) || !asOwner && PhotonNetwork.AllocateSceneViewID(view)) {
                    //Add this to the instance tracking
                    instancePool.Add(view.ViewID, new NetworkInstance(view.ViewID, go));
                    //Notify others to spawn this object using the provided settings
                    NetworkInstantiateNotify(receivers, eventCacheOption, SpawnObjectCode, view.ViewID, prefabIndex);
                    return go;
                } else {
                    Debug.LogFormat("<Color=Red>Failed to allocate a ViewId while instantiating game object {0}.</Color>", go.name);
                }
            } else {
                Debug.LogFormat("<Color=Red>Attempting to instantiate gameobject {0} that is missing the photon view component. Please add it to the prefab of that object.</Color>", go.name);
            }
            Destroy(go);
            return null;
        }
        
        //Notifys others to spawn this object
        private void NetworkInstantiateNotify ( ReceiverGroup receivers, EventCaching eventCacheOption, byte eventCode, int viewID, int prefabIndex ) {
            object[] data = GenerateSpawnData(viewID, prefabIndex);
            RaiseEventOptions eventOptions = new RaiseEventOptions {
                Receivers = receivers,
                CachingOption = eventCacheOption
            };
            SendOptions sendOptions = new SendOptions {
                Reliability = true
            };
            PhotonNetwork.RaiseEvent(eventCode, (object) data, eventOptions, sendOptions);
        }

        private static object[] GenerateSpawnData(int viewID, int prefabIndex) {
            return GenerateSpawnData(viewID, prefabIndex, Vector3.zero, Quaternion.identity);
        }

        private static object[] GenerateSpawnData(int viewID, int prefabIndex, Vector3 position, Quaternion rotation) {

            if (position != null && rotation == null) {
                return new object[] { viewID, prefabIndex, position };
            } else if (position != null && rotation != null) {
                return new object[] { viewID, prefabIndex, position, rotation };
            }

            return new object[] { viewID, prefabIndex };
        }

        private struct NetworkInstance {
            public int viewID;
            public GameObject gameObjectRef;

            public NetworkInstance(int viewID, GameObject go) {
                this.viewID = viewID;
                gameObjectRef = go;
            }
        }
    }
}
