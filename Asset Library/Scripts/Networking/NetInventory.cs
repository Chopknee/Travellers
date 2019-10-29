using Photon.Pun;
using System.Collections.Generic;
using BaD.Modules.Networking;
using UnityEngine;
using System.Linq;

namespace BaD.Modules.Networking {

    public class NetInventory: Messaging {

        private const byte RequestSyncCode = 0;
        private const byte InventorySync = 2;
        private const byte TakeItemCode = 1;
        private const byte AddItemCode = 3;
        private const byte RequestFailed = 4;

        public delegate void ItemsUpdated ( int originalRequestID, ItemInstance[] addedItems, ItemInstance[] removedItems );
        public ItemsUpdated OnItemsUpdated;

        public delegate void ItemRemovedResponse ( bool success );

        public delegate void InventoryRequestCallback ( int ogRequestId, bool itemsTaken, bool success, ItemInstance[] items );
        Dictionary<int, InventoryRequest> requestCallbacks;

        public ItemInstance[] Items {
            get {
                return items.ToArray();
            }
        }

        private List<ItemInstance> items;//This is for the clients only, it is not a master list.
        private List<ItemInstance> masterItemsList;//This is unused by anyone but the master.

        private new void Awake () {
            base.Awake();
            items = new List<ItemInstance>();
            masterItemsList = new List<ItemInstance>();

            if (PhotonNetwork.IsMasterClient) {
                PhotonNetwork.AddCallbackTarget(this);
            }

            requestCallbacks = new Dictionary<int, InventoryRequest>();
        }

        //Before doing anything with this inventory, it must be opened
        public void Open () {
            if (!PhotonNetwork.IsMasterClient) {//The master client will have to be always on.
                PhotonNetwork.AddCallbackTarget(this);
            }
            RequestItemsSync();
        }

        //When finished performing operations on this inventory, it must be closed.
        public void Close () {
            if (!PhotonNetwork.IsMasterClient) {
                PhotonNetwork.RemoveCallbackTarget(this);
            }
        }

        public int RemoveItem ( ItemInstance i, InventoryRequestCallback callback = null ) {
            return RemoveItems(new ItemInstance[] { i }, callback);
        }

        public int RemoveItem ( ItemType i, InventoryRequestCallback callback = null ) {
            ItemInstance netI = null;
            //This gets a little more complicated, as I need to find the item specifically to remove (assuming one even can be).
            //For each network item in the inventory
            foreach (ItemInstance it in items) {
                //If the current item's type is equivalent to the provided item type
                if (NetworkedInventoryManager.Instance.Compare(it, i)) {
                    //Select it and break the loop
                    netI = it;
                    break;
                }
            }
            //If the specific item instance is not null
            if (netI != null) {
                //Send a command to have it removed immediately
                return RemoveItem(netI, callback);
            } else {
                return -1;//Failed to find an item matching.
            }
        }

        //Removes an item from this inventory
        public int RemoveItems ( ItemInstance[] i, InventoryRequestCallback callback = null ) {
            object[] data = new object[] { TakeItemCode, i };
            int id = SendNetMessage(data);
            if (callback != null) {
                requestCallbacks.Add(id, new InventoryRequest(callback, id, true, i));
            }
            return id;
        }

        public int AddItem ( ItemInstance i, InventoryRequestCallback callback = null ) {
            return AddItems(new ItemInstance[] { i }, callback);
        }

        //Add the requested items to the inventory
        public int AddItems ( ItemInstance[] i, InventoryRequestCallback callback = null ) {
            
            object[] data = new object[] { AddItemCode, i };
            int id = SendNetMessage(data);

            if (callback != null) {
                requestCallbacks.Add(id, new InventoryRequest(callback, id, false, i));
            }

            return id;
        }

        //Simply requests to syncronize the inventory
        private int RequestItemsSync () {
            object[] data = new object[] { RequestSyncCode };
            return SendNetMessage(data);
        }

        //A reuqest to sync the inventory with others has been received.
        private int SyncOpenedInventories ( int originatingRequestID ) {
            object[] data = new object[] { InventorySync, masterItemsList.ToArray(), originatingRequestID };
            return SendNetMessage(data);
        }

        //When a inventory sync request has been fulfilled on the client end
        private void SyncInventoryResponse ( ItemInstance[] newItems, int originalRequestID ) {
            //Added items are items that are in the new items list, but not in the old items list
            //Removed items are items that are not in the new items list, but are in the old items list
            List<ItemInstance> removed = new List<ItemInstance>();
            removed.AddRange(items);
            List<ItemInstance> added = new List<ItemInstance>();
            added.AddRange(newItems);
            foreach (ItemInstance newI in newItems) {
                foreach (ItemInstance oldI in items) {
                    if (newI == oldI) {
                        removed.Remove(newI);
                        added.Remove(newI);
                    }
                }
            }


            items.Clear();
            items.AddRange(newItems);
            if (requestCallbacks.ContainsKey(originalRequestID)) {
                Debug.Log("Running request callback!");
                InventoryRequest req = requestCallbacks[originalRequestID];
                requestCallbacks.Remove(originalRequestID);
                req.callback?.Invoke(originalRequestID, req.itemsTaken, true, req.items);
            }
            Debug.LogFormat("Inventory {2} had {0} items removed and {1} items added.", removed.Count(), added.Count(), gameObject.name);
            //Always invoke this, because even the master will make requests.
            OnItemsUpdated?.Invoke(originalRequestID, added.ToArray(), removed.ToArray());
        }

        //This is only run by the master client.
        private void TakeItemRequestReceived ( ItemInstance[] items, int originalRequestID ) {

            //This is just to check if all the items being removed are in this inventory to begin with
            bool canTake = true;
            List<int> indices = new List<int>();
            foreach (ItemInstance i in items) {
                int index = FindItemIndexByNetworkId(i);
                //Ignore requests for items not in this inventory
                canTake &= index != -1;
                if (canTake == false)
                    break;
                indices.Add(index);
            }

            //If at least one of the items are missing, the request will fail and the originating player should be alerted
            if (!canTake) {
                SendNetMessage(new object[] { RequestFailed, originalRequestID });
                return;
            }

            //Now actullay remove the items from this inventory
            foreach (int index in indices) {
                masterItemsList.RemoveAt(index);
            }

            SyncOpenedInventories(originalRequestID);//Syncronize all opened instances of this inventory.
        }

        //This is only run by the master client.
        private void AddItemsRequestReceived ( ItemInstance[] items, int originalRequestID ) {
            //Debug.Log("Adding item(s) " + items.Length);
            foreach (ItemInstance i in items) {
                int index = FindItemIndexByNetworkId(i);
                //Ignore requests for items already in this inventory
                if (index == -1) {//If the item is already in the inventory, don't add it again.
                    masterItemsList.Add(i);

                }
            }
            SyncOpenedInventories(originalRequestID);//Syncronize all opened instances of this inventory.
        }

        public override void MessageReceived ( object[] data ) {
            MessageMeta messageMeta = (MessageMeta) data[0];

            //Get the subcode from the data, this determines what we do with the received data.
            byte subcode = (byte) data[1];
            //Debug.LogFormat("Got a message; Message ID: {0}, Received View ID: {1}, Subcode: {2}, My View ID: {3} ", messageMeta.MessageID, messageMeta.ViewID, SubcodeToString(subcode), ViewID);
            switch (subcode) {//Sub-codes to avoid cluttering the main command code space.
                case RequestSyncCode:
                    if (PhotonNetwork.IsMasterClient) {
                        SyncOpenedInventories(messageMeta.MessageID);
                    }
                    break;
                case InventorySync:
                    ItemInstance[] items = (ItemInstance[]) data[2];
                    SyncInventoryResponse(items, (int) data[3]);
                    break;
                case TakeItemCode:
                    if (PhotonNetwork.IsMasterClient) {
                        ItemInstance[] ir = (ItemInstance[]) data[2];
                        TakeItemRequestReceived(ir, messageMeta.MessageID);
                    }
                    break;
                case AddItemCode:
                    if (PhotonNetwork.IsMasterClient) {
                        ItemInstance[] ia = (ItemInstance[]) data[2];
                        AddItemsRequestReceived(ia, messageMeta.MessageID);
                    }
                    break;
                case RequestFailed:
                    //If a request fails, then run the request callback if one was given.
                    if (requestCallbacks.ContainsKey((int)data[2])) {
                        InventoryRequest req = requestCallbacks[(int)data[2]];
                        requestCallbacks.Remove(req.ogRequestId);
                        req.callback?.Invoke(req.ogRequestId, req.itemsTaken, false, req.items);
                    }
                    break;
            }
        }

        public string SubcodeToString ( int subcode ) {
            switch (subcode) {
                case RequestSyncCode:
                    return "Request Sync";
                case InventorySync:
                    return "Inventory Sync";
                case TakeItemCode:
                    return "Take Item";
                case AddItemCode:
                    return "Add Item";
                default:
                    return "Unknown";
            }
        }

        public int FindItemIndexByNetworkId ( ItemInstance item ) {
            for (int i = 0; i < items.Count; i++) {
                if (items[i].networkID == item.networkID)
                    return i;
            }
            return -1;
        }

        struct InventoryRequest {

            public InventoryRequestCallback callback;
            public int ogRequestId;
            public bool itemsTaken;
            public ItemInstance[] items;

            public InventoryRequest(InventoryRequestCallback callback, int ogId, bool itemsTaken, ItemInstance[] items) {
                this.callback = callback;
                this.ogRequestId = ogId;
                this.itemsTaken = itemsTaken;
                this.items = items;
            }
        }

    }
}