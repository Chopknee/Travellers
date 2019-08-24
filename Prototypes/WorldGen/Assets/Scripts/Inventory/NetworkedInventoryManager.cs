using BaD.Chopknee.Utilities;
using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaD.Modules.Networking {
    public class NetworkedInventoryManager: Messaging {

        public static NetworkedInventoryManager Instance { get; private set; }

        public int RequestCacheLengthForInventories = 100;

        public Dictionary<int, GameObject> masterInventories;
        public Dictionary<int, GameObject> inventories = new Dictionary<int, GameObject>();

        public delegate void InventoryRequestCallback ( GameObject reference );
        public Dictionary<int, InventoryRequestCallback> requestCallbacks = new Dictionary<int, InventoryRequestCallback>();

        private int nextNetworkItemId = 0;

        [SerializeField]
#pragma warning disable 0649
        private ItemCard[] NetworkedItems;//All items that can exist in an inventory.

        public new void Awake () {
            base.Awake();
            Instance = this;
            PhotonPeer.RegisterType(typeof(Item), (byte) 'I', Item.Serialize, Item.DeSerialize);
            if (PhotonNetwork.IsMasterClient) {
                masterInventories = new Dictionary<int, GameObject>();
            }
        }

        //This is sent to the master to get a new inventory object spawned
        public void RequestInventory ( int id, InventoryRequestCallback onRequestFulfilled ) {
            if (!inventories.ContainsKey(id)) {
                int messID = SendNetMessage(new object[] { (byte) 0, id });
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
        private void InventoryRequestResponseResponse ( int messageID, int id, int viewID ) {
            if (!inventories.ContainsKey(id)) {
                GameObject invGO = SpawnInventory(id, ViewID);
                inventories.Add(id, invGO);
                if (requestCallbacks.ContainsKey(messageID)) {
                    requestCallbacks[messageID]?.Invoke(invGO);
                    requestCallbacks.Remove(messageID);
                }
            }
        }

        public override void MessageReceived ( object[] messageData ) {
            switch ((byte) messageData[1]) {
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

        public ItemCard GetItemData(Item i) {//Takes the network item reference, and converts it to item data
            return NetworkedItems[i.itemIndex];
        }

        public Item MakeItemStruct(ItemCard ic) {
            int ind = GetItemCardIndex(ic);
            if (ind != -1) {
                return new Item((short)ind);
            }
            throw new Exception("Item requested was not added to networked items list in Networked Inventory Manager Object.");
        }

        public ItemCard[] GetItemData(Item[] networkItems) {
            ItemCard[] items = new ItemCard[networkItems.Length];
            for (int i = 0; i < networkItems.Length; i++) {
                items[i] = GetItemData(networkItems[i]);
            }
            return items;
        }

        public int GetItemCardIndex(ItemCard ic ) {
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
    }

    [Serializable]
    public class Item : IEquatable<Item> {
        public short itemIndex { get; }//This links to one of the items int he ItemCard array NetowrkedItems
        public short networkID { get; }//This is unique to this item

        public Item ( short itemCardIndex ) {
            itemIndex = itemCardIndex;
            networkID = 0;
            if (NetworkedInventoryManager.Instance != null) {
                networkID = (short)NetworkedInventoryManager.Instance.GetItemNetworkId();
            }
        }

        private Item(short itemCardIndex, short netId) {
            itemIndex = itemCardIndex;
            networkID = netId;
        }

        public static byte[] Serialize(object received) {
            byte[] arr = new byte[4];
            Item item = (Item) received;
            Choptilities.ShortToBytes(item.itemIndex, out arr[0], out arr[1]);
            Choptilities.ShortToBytes(item.networkID, out arr[2], out arr[3]);
            return arr;
        }

        public static object DeSerialize(byte[] received) {
            Item i = new Item(
                Choptilities.ByteToShort(received[0], received[1]),
                Choptilities.ByteToShort(received[2], received[3])
                );
            return i;
        }

        public bool Equals ( Item i ) {
            return itemIndex == i.itemIndex && networkID == i.networkID;
        }
    }
}
