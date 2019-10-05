using BaD.Modules.Networking;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetInventoryManager : Messaging {

    public static NetInventoryManager Instance { get; private set; }

    public int RequestCacheLengthForInventories = 100;

    public Dictionary<int, GameObject> masterInventories;
    public Dictionary<int, GameObject> inventories = new Dictionary<int, GameObject>();

    public delegate void InventoryRequestCallback ( GameObject reference );
    public Dictionary<int, InventoryRequestCallback> requestCallbacks = new Dictionary<int, InventoryRequestCallback>();

    private new void Awake () {
        base.Awake();
        Instance = this;
    }

    //This is sent to the master to get a new inventory object spawned
    public void RequestInventory(int id, InventoryRequestCallback onRequestFulfilled) {
        if (!inventories.ContainsKey(id)) {
            int messID = SendNetMessage(new object[] { (byte)0, id });
            requestCallbacks.Add(messID, onRequestFulfilled);
        } else {
            onRequestFulfilled?.Invoke(inventories[id]);
        }
    }

    //This is the response to the request from the server to the clients
    private void InventoryRequestResponse ( int id ) {
        GameObject invgo;
        if (masterInventories.ContainsKey(id)) {
            //Respond with the spawn data from the existing object
            invgo = masterInventories[id];
        } else {
            invgo = SpawnInventory(id);
            masterInventories.Add(id, invgo);
        }
        SendNetMessage(new object[] { (byte) 1, id, invgo.GetComponent<PhotonView>().ViewID });
    }

    //This is the response of the client to the message from the server with the inventory view id
    private void InventoryRequestResponseResponse(int messageID, int id, int viewID) {
        if (!inventories.ContainsKey(id)) {
            GameObject invGO = SpawnInventory(id, ViewID);
            inventories.Add(id, invGO);
            if (!requestCallbacks.ContainsKey(messageID)) {
                requestCallbacks[messageID]?.Invoke(invGO);
                requestCallbacks.Remove(messageID);
            }
        }
    }

    public override void MessageReceived ( object[] messageData ) {
        switch ((byte)messageData[1]) {
            case 0:
                //Requesting an inventory
                if (PhotonNetwork.IsMasterClient) {
                    //Fulfill the request.
                    InventoryRequestResponse((int) messageData[2]);
                }
                break;
            case 1:
                MessageMeta mm = (MessageMeta) messageData[0];
                InventoryRequestResponseResponse(mm.MessageID, (int) messageData[2], (int) messageData[3]);
                break;
        }

        //throw new System.NotImplementedException();
    }



    public GameObject SpawnInventory (int id, int viewID = -1) {
        GameObject invGo = new GameObject("Inventory" + id);
        invGo.transform.parent = transform.parent;
        PhotonView view = invGo.AddComponent<PhotonView>();//The view id will be a thinggggg....
        if (viewID == -1) {
            //Generating a new view id
            if (!PhotonNetwork.AllocateSceneViewID(view)) {
                return null;
            }
        } else {
            //Setting based on an existing view id
            view.ViewID = ViewID;
        }
        //Then we have a successful instantiation of the game object... continue
        NetInventory inv = invGo.AddComponent<NetInventory>();
        inv.MessageCode = 10;
        inv.RequestNumberCacheLength = RequestCacheLengthForInventories;
        inv.FilterMessagesByView = true;
        inv.Receivers = Photon.Realtime.ReceiverGroup.All;
        inv.CachingOption = Photon.Realtime.EventCaching.DoNotCache;
        inv.ReliabilityMode = true;
        return invGo;
        //ph.ViewID
    }
}