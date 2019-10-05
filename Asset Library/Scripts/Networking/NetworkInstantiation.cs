using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaD.Modules.Networking {
    /// <summary>
    /// Class that handles instantiation of gameobjects via RPC messages.
    /// This is a replacement for Photon's poor excuse for network instantiation.
    /// </summary>
    public class NetworkInstantiation: Messaging {

        /// <summary>
        /// The first instance of this object that was loaded in will be stored here.
        /// </summary>
        public static NetworkInstantiation Instance { get; private set; }

        /// <summary>
        /// This is a list of objects that can be spawned via the network. All clients should have
        /// this same list.
        /// </summary>
        [SerializeField]
#pragma warning disable 0649
        private GameObject[] spawnablesPool;

        /// <summary>
        /// This is a dictionary of all currently live network gameobject instances.
        /// </summary>
        public Dictionary<int, NetworkInstance> instancePool = new Dictionary<int, NetworkInstance>();

        /// <summary>
        /// Returns an array of all objects that can be spawned via the network.
        /// </summary>
        public GameObject[] SpawnablesPool {
            get {
                return spawnablesPool;
            }
        }

        public override void Awake() {
            base.Awake();
            if (Instance == null) {//Instance will only ever be equal to the first occurance of this object.
                Instance = this;
            }
        }

        public override void MessageReceived ( object[] messageData ) {
            MessageMeta messageMeta = (MessageMeta) messageData[0];
            int viewID = (int) messageData[1];
            int prefabPoolIndex = (int) messageData[2];

            if (!instancePool.ContainsKey(viewID)) {
                GameObject go = Instantiate(spawnablesPool[prefabPoolIndex], (Vector3) messageData[3], (Quaternion) messageData[4]);
                PhotonView view = go.GetComponent<PhotonView>();
                view.ViewID = viewID;
                instancePool.Add(view.ViewID, new NetworkInstance(view.ViewID, go));
            }
        }

        public GameObject Instantiate(GameObject prefab, bool asOwner) {
            return Instantiate(prefab, asOwner, Vector3.zero, Quaternion.identity);
        }

        public GameObject Instantiate ( GameObject prefab, bool asOwner, Vector3 position, Quaternion rotation ) {
            int prefabIndex = GetSpawnableIndex(prefab);
            if (prefabIndex == -1) {
                Debug.LogFormat("<Color=Red>Failed to network instantiate game object {0}, it was not in the spawn pool.</Color>", prefab.name);
                return null;
            }

            if (prefabIndex >= spawnablesPool.Length) {
                Debug.LogFormat("<Color=Red>Failed to network instantiate object, prefab index {0} does not exist.</Color>", prefabIndex);
                return null;
            }

            GameObject go = Instantiate(spawnablesPool[prefabIndex], position, rotation);
            PhotonView view = go.GetComponent<PhotonView>();

            if (view != null) { 
                //Basically; before the ||, if as owner is false, the second statement is not executed and skips to after the ||
                if ( asOwner && PhotonNetwork.AllocateViewID(view) || !asOwner && PhotonNetwork.AllocateSceneViewID(view)) {
                    //Add this to the instance tracking
                    instancePool.Add(view.ViewID, new NetworkInstance(view.ViewID, go));
                    //Notify others to spawn this object using the provided settings
                    SendNetMessage(new object[] { view.ViewID, prefabIndex, position, rotation });
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

        public int GetSpawnableIndex(GameObject go) {
            int ind = -1;
            for (int i = 0; i < spawnablesPool.Length; i++) {
                if (go == spawnablesPool[i]) {
                    ind = i;
                    break;
                }
            }
            if (ind == -1) {
                Debug.LogErrorFormat("Attempting to find the index of gameobject {0} that has not been added to the spawnables pool.", go.name);
            }
            return ind;
        }

        public struct NetworkInstance {

            public int viewID { get; private set; }
            public GameObject gameObjectRef;

            public NetworkInstance(int viewID, GameObject go) {
                this.viewID = viewID;
                gameObjectRef = go;
            }
        }
    }
}
