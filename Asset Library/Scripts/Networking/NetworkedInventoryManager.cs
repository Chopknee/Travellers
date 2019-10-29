using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaD.Modules.Networking {

    public class NetworkedInventoryManager: Messaging {

        private const byte RequestInventoryCode = 4;
        private const byte InventoryRequestRecCode = 2;

        public static NetworkedInventoryManager Instance { get; private set; }

        public int RequestCacheLengthForInventories = 100;

        public Dictionary<int, GameObject> masterInventories;
        public Dictionary<int, GameObject> inventories = new Dictionary<int, GameObject>();

        public delegate void InventoryRequestCallback ( GameObject reference, bool needsInitialize );
        public Dictionary<int, InventoryRequestCallback> requestCallbacks = new Dictionary<int, InventoryRequestCallback>();

        private int nextNetworkItemId = 0;

        [SerializeField]
#pragma warning disable 0649
        private ItemType[] NetworkedItems;//All items that can exist in an inventory.

        public new void Awake () {
            base.Awake();
            Instance = this;
            PhotonPeer.RegisterType(typeof(ItemInstance), (byte) 'I', ItemInstance.Serialize, ItemInstance.DeSerialize);
            if (PhotonNetwork.IsMasterClient) {
                masterInventories = new Dictionary<int, GameObject>();
            }
        }

        //This is sent to the master to get a new inventory object spawned
        public void RequestInventory ( int id, InventoryRequestCallback onRequestFulfilled ) {
            if (!inventories.ContainsKey(id)) {
                Debug.Log("Sending a request to master for an inventory.");
                int messID = SendNetMessage(new object[] { RequestInventoryCode, id });
                requestCallbacks.Add(messID, onRequestFulfilled);
                
            } else {
                Debug.Log("Inventory was already initialized, returning!");
                onRequestFulfilled?.Invoke(inventories[id], false);
            }
        }

        //This is the response to the request from the server to the clients
        private void InventoryRequestResponse ( int id, int originatingMessageID ) {
            Debug.Log("<color=red>Inventory request has been received by master.</color>");
            bool existingAlready = false;
            GameObject invgo;
            if (masterInventories.ContainsKey(id)) {
                //Respond with the spawn data from the existing object
                invgo = masterInventories[id];
                existingAlready = true;
            } else {
                invgo = SpawnInventory(id);
                masterInventories.Add(id, invgo);
            }
            SendNetMessage(new object[] { InventoryRequestRecCode, id, invgo.GetComponent<PhotonView>().ViewID, !existingAlready, originatingMessageID });
        }

        //This is the response of the client to the message from the server with the inventory view id
        private void InventoryRequestResponseResponse ( int messageID, int id, int viewID, bool shouldInitialize ) {
            Debug.Log("Received a response to generate a new inventory/create and existing one.");
            if (!inventories.ContainsKey(id)) {
                GameObject invGO = SpawnInventory(id, viewID);
                inventories.Add(id, invGO);
                if (requestCallbacks.ContainsKey(messageID)) {
                    requestCallbacks[messageID]?.Invoke(invGO, shouldInitialize);
                    requestCallbacks.Remove(messageID);
                }
            } else {
                Debug.Log("Received a request to spawn an existing inventory. (Not intended function!)");
            }
        }

        public override void MessageReceived ( object[] messageData ) {
            
            MessageMeta mm = (MessageMeta) messageData[0];

            //Debug.Log("Got a message for an inventory... " + (byte)messageData[1]);
            switch ((byte) messageData[1]) {
                case RequestInventoryCode:
                    Debug.Log("Received a request for a new inventory.");
                    //Requesting an inventory
                    if (PhotonNetwork.IsMasterClient) {
                        //Fulfill the request.
                        InventoryRequestResponse((int) messageData[2], mm.MessageID);
                    }
                    break;
                case InventoryRequestRecCode:
                    InventoryRequestResponseResponse((int)messageData[5], (int) messageData[2], (int) messageData[3], (bool) messageData[4]);
                    Debug.Log("Inventory request response.. response has been received.");
                    break;
                default:
                    Debug.Log("INVALID MESSAGE CODE IN INVENTORY MANAGER.");
                    break;
            }

            //throw new System.NotImplementedException();
        }



        public GameObject SpawnInventory ( int id, int viewID = -1 ) {
            if (PhotonNetwork.IsMasterClient && viewID != -1) {
                //Then I get my reference from the master list
                return masterInventories[id];//Maybe this will work.
            }
            GameObject invGo = new GameObject("Inventory" + id);
            invGo.transform.SetParent(transform);
            PhotonView view = invGo.AddComponent<PhotonView>();//The view id will be a thinggggg....
            if (viewID == -1) {
                //Generating a new view id
                if (!PhotonNetwork.AllocateSceneViewID(view)) {
                    return null;
                }
            } else {
                //Setting based on an existing view id
                view.ViewID = viewID;
            }
            //Then we have a successful instantiation of the game object... continue
            NetInventory inv = invGo.AddComponent<NetInventory>();
            inv.MessageCode = 10;
            inv.RequestNumberCacheLength = RequestCacheLengthForInventories;
            inv.FilterMessagesByView = true;
            inv.Receivers = Photon.Realtime.ReceiverGroup.All;
            inv.CachingOption = Photon.Realtime.EventCaching.DoNotCache;
            inv.ReliabilityMode = true;
            inv.Awake();
            return invGo;
            //ph.ViewID
        }

        public ItemType GetItemData(ItemInstance i) {//Takes the network item reference, and converts it to item data
            return NetworkedItems[i.itemIndex];
        }

        public ItemInstance MakeItemStruct(ItemType ic) {
            int ind = GetItemDataIndex(ic);
            if (ind != -1) {
                return new ItemInstance((short)ind);
            }
            throw new Exception("Item requested was not added to networked items list in Networked Inventory Manager Object.");
        }

        public ItemType[] GetItemData(ItemInstance[] networkItems) {
            ItemType[] items = new ItemType[networkItems.Length];
            for (int i = 0; i < networkItems.Length; i++) {
                items[i] = GetItemData(networkItems[i]);
            }
            return items;
        }

        public int GetItemDataIndex(ItemType ic ) {
            for (int i = 0; i < NetworkedItems.Length; i++) { 
                if (NetworkedItems[i] == ic) {
                    return i;
                }
            }
            return -1;
        }

        public int GetItemNetworkId() {//Limiting, but should do the trick for getting unique numbers between 4 players possibly trying to all spawn things syncronously
            int nId = nextNetworkItemId + (PhotonNetwork.LocalPlayer.ActorNumber*1000);
            nextNetworkItemId++;
            return nId;
        }

        public bool Compare(ItemInstance netItem, ItemType itemData) {
            return netItem.itemIndex == GetItemDataIndex(itemData);
        }
    }
}
