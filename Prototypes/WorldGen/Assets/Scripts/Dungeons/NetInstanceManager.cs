using BaD.Modules.Networking;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetInstanceManager: Messaging {

    public static NetInstanceManager CurrentManager { get; private set; }

    public int instanceID { get; private set;}
    public bool isInstanceMaster;

    //Some sub-code information;
    //  byte 0 - 
    private const byte JoinInstanceNotificationCode = 0;
    private const byte SpawnInstance = 1;
    private const byte NotMaster = 2;
    private const byte LeaveInstanceCode = 3;
    private const byte InstantiateRequest = 4;
    private const byte DestroyGORequest = 5;

    [SerializeField]
#pragma warning disable 0649
    private GameObject[] spawnablesPool;


    public Dictionary<int, NetworkInstance> instancesPool = new Dictionary<int, NetworkInstance>();
    public List<int> joinedPlayers = new List<int>();

    public static void CloneSettings(NetInstanceManager from, NetInstanceManager to) {
        //Copying only the settings here.
        to.MessageCode = from.MessageCode;
        to.RequestNumberCacheLength = from.RequestNumberCacheLength;
        to.FilterMessagesByView = from.FilterMessagesByView;
        to.Receivers = from.Receivers;
        to.CachingOption = from.CachingOption;
        to.ReliabilityMode = from.ReliabilityMode;
        to.spawnablesPool = from.spawnablesPool;
    }

    public new void Awake () {
        base.Awake();
        enabled = false;
    }

    public void JoinInstance(int instanceID) {
        Debug.LogFormat("Joining instance <color=green>{0}</color>.", instanceID);
        this.instanceID = instanceID;
        object[] data = new object[] { instanceID, JoinInstanceNotificationCode, PhotonNetwork.LocalPlayer.ActorNumber };
        SendNetMessage(data);
        Debug.LogFormat("Sent dungeon instance join notification <color=green>{0}</color>.", instanceID);
        isInstanceMaster = true;
        CurrentManager = this;
    }

    public void LeaveInstance() {
        //If I'm the instance master, i need to pick the next player as the master
        if (joinedPlayers.Count > 0) {//Only send the message if there are other players in the session
            if (isInstanceMaster) {
                //Send my next choice of player to be the master
                SendNetMessage(new object[] { instanceID, LeaveInstanceCode, isInstanceMaster, PhotonNetwork.LocalPlayer.ActorNumber, joinedPlayers[0] });
                //Move the ownership of all objects to the newly selected player
                foreach (KeyValuePair<int, NetworkInstance> entry in instancesPool) {
                    if (entry.Value.gameObjectRef.GetComponent<PhotonView>().IsMine) {
                        entry.Value.gameObjectRef.GetComponent<PhotonView>().TransferOwnership(joinedPlayers[0]);
                    }
                }
            } else {
                //Just send my actor number so the others can remove me
                SendNetMessage(new object[] { instanceID, LeaveInstanceCode, isInstanceMaster, PhotonNetwork.LocalPlayer.ActorNumber });
            }
            
        }

        foreach (KeyValuePair<int, NetworkInstance> entry in instancesPool) {
            Destroy(entry.Value.gameObjectRef);
        }
        instancesPool.Clear();
        if (CurrentManager == this) {
            CurrentManager = null;
        }
    }

    public void DestroyObject(GameObject go) {
        int ind = -1;
        foreach (KeyValuePair<int, NetworkInstance> kvp in instancesPool) {
            if (kvp.Value.gameObjectRef == go) {
                ind = kvp.Key;
                break;
            }
        }
        
        SendNetMessage(new object[] { instanceID, DestroyGORequest, ind });
        instancesPool.Remove(ind);
        Destroy(go);
    }

    private void OnDestroyReceived(int ind) {
        if (instancesPool.ContainsKey(ind)) {
            Destroy(instancesPool[ind].gameObjectRef);
            instancesPool.Remove(ind);
        }
    }

    public override void MessageReceived ( object[] messageData ) {
        //Message received stuff here...
        MessageMeta meta = (MessageMeta) messageData[0];
        Debug.LogFormat("RECEIVED FUCKING MESSAGE Receiver ID:{0}, Message Code: {1}, {2}", (int) messageData[1], (byte) messageData[2], ((int) messageData[1] != instanceID)? "Not For Me":"For Me");
        //If message is not related to this instance, ignore it
        if ((int)messageData[1] != instanceID) { return; }
        switch ((byte) messageData[2]) {
            case JoinInstanceNotificationCode:
                //Join instance was received.
                joinedPlayers.Add((int) messageData[3]);//track the players currently joined.
                Debug.LogFormat("A new player  <color=green>{1}</color> has joined instance <color=green>{0}</color>.", instanceID, messageData[3]);
                if (isInstanceMaster) {
                    Debug.LogFormat("I am the master in instance <color=green>{0}</color> so I tell the player to EFF off.", instanceID);
                    //Basically tell them they aren't the master here.
                    SendNetMessage(new object[] { instanceID, NotMaster, joinedPlayers.ToArray() });
                    //And then sync all the networked objects in the instance
                    foreach (KeyValuePair<int, NetworkInstance> entry in instancesPool) {
                        //Send to the user to spawn this object.
                        GameObject go = entry.Value.gameObjectRef;
                        SendNetMessage(new object[] { instanceID, SpawnInstance, entry.Key, entry.Value.prefabIndex, go.transform.position, go.transform.rotation });
                    }
                }
                break;
            case SpawnInstance://Synchronization of network instances
                if (instancesPool.ContainsKey((int) messageData[3])) { return; }//Don't spawn this if it has already been spawned
                GameObject inst = Instantiate(spawnablesPool[(int) messageData[4]], (Vector3) messageData[5], (Quaternion) messageData[6]);
                PhotonView view = inst.GetComponent<PhotonView>();
                view.ViewID = (int) messageData[3];
                instancesPool.Add(view.ViewID, new NetworkInstance(view.ViewID, (int)messageData[4], inst));
                break;
            case NotMaster:
                Debug.LogFormat(" I received a not master notification for the current instance {0}.", instanceID);
                //A special message that is received when joining an instance and someone else has already made it
                isInstanceMaster = false;
                //Also, sync the joined players list.
                int[] pls = (int[]) messageData[3];
                joinedPlayers.AddRange(pls);

                break;
            case LeaveInstanceCode:
                //No matter what, remove the leaving player form the list.
                int leavingPlayer = (int) messageData[4];
                for (int i = 0; i < joinedPlayers.Count; i++) {
                    if (joinedPlayers[i] == leavingPlayer) {
                        joinedPlayers.RemoveAt(i);
                        break;//Breaking from the loop.
                    }
                }
                if ((bool)messageData[3]) {//This tells if the one that left was the instance master??
                    //Master left
                    if ((int)messageData[5] == PhotonNetwork.LocalPlayer.ActorNumber) {
                        //Set myself as the new master
                        isInstanceMaster = true;
                    }
                }
                break;
            case InstantiateRequest:
                if (isInstanceMaster) {
                    //Got a request to instantiate something, do it now.
                    Instantiate(spawnablesPool[(int)messageData[3]], true, (Vector3)messageData[4], (Quaternion) messageData[5]);
                }
                break;
            case DestroyGORequest:
                OnDestroyReceived((int) messageData[3]);
                break;
        }
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
            if ((asOwner || !asOwner && isInstanceMaster)) {
                if (PhotonNetwork.AllocateViewID(view)) {
                    //Add this to the instance tracking
                    instancesPool.Add(view.ViewID, new NetworkInstance(view.ViewID, prefabIndex, go));
                    //Notify others to spawn this object using the provided settings
                    SendNetMessage(new object[] { instanceID, SpawnInstance, view.ViewID, prefabIndex, position, rotation });
                    return go;
                } else {
                    Debug.LogFormat("<Color=Red>Failed to allocate a ViewId while instantiating game object {0}.</Color>", go.name);
                }
            } else {
                //We don't want to be the owner in this case, request the master to instantiate the object
                SendNetMessage(new object[] { instanceID, InstantiateRequest, prefabIndex, position, rotation });
            }
        } else {
            Debug.LogFormat("<Color=Red>Attempting to instantiate gameobject {0} that is missing the photon view component. Please add it to the prefab of that object.</Color>", go.name);
        }
        Destroy(go);
        return null;
    }

    public int GetSpawnableIndex ( GameObject go ) {
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
        public int prefabIndex { get; private set; }

        public NetworkInstance ( int viewID, int prefabIndex, GameObject instance ) {
            this.viewID = viewID;
            this.prefabIndex = prefabIndex;
            gameObjectRef = instance;
        }
    }
}
