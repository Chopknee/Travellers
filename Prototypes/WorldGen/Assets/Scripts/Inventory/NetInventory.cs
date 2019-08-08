using Photon.Pun;
using System.Collections.Generic;
using BaD.Modules.Networking;
using UnityEngine;
using System.Linq;

public class NetInventory : Messaging {

    private const byte RequestSyncCode = 0;
    private const byte InventorySync = 2;
    private const byte TakeItemCode = 1;
    private const byte AddItemCode = 3;

    public delegate void ItemsUpdated(int originalRequestID, Item[] addedItems, Item[] removedItems);
    public ItemsUpdated OnItemsUpdated;

    public delegate void ItemRemovedResponse ( bool success );

    public Item[] Items {
        get {
            return items.ToArray();
        }
    }

    private List<Item> items;//This is for the clients only, it is not a master list.
    private List<Item> masterItemsList;//This is unused by anyone but the master.

    private new void Awake () {
        base.Awake();
        items = new List<Item>();
        masterItemsList = new List<Item>();

        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.AddCallbackTarget(this);
        }
    }

    //Before doing anything with this inventory, it must be opened
    public void Open() {
        if (!PhotonNetwork.IsMasterClient) {//The master client will have to be always on.
            PhotonNetwork.AddCallbackTarget(this);
        }
        RequestItemsSync();
    }

    //When finished performing operations on this inventory, it must be closed.
    public void Close() {
        if (!PhotonNetwork.IsMasterClient) {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
    }

    public int RemoveItem ( Item i ) {
        return RemoveItems(new Item[] { i });
    }

    //Removes an item from this inventory
    public int RemoveItems( Item[] i ) {
        //Remove the requested item from the inventory
        object[] data = new object[] { TakeItemCode, i };
        return SendNetMessage(data);
    }

    public int AddItem(Item i) {
        return AddItems(new Item[] { i });
    }

    public int AddItems(Item[] i) {
        //Add the requested item to the inventory
        object[] data = new object[] { AddItemCode, i };
        return SendNetMessage(data);
    }

    private int RequestItemsSync() {
        //
        object[] data = new object[] { RequestSyncCode };
        return SendNetMessage(data);
    }

    private int SyncOpenedInventories ( int originatingRequestID ) {

        //A reuqest to synch the inventory with others has been received.
        object[] data = new object[] { InventorySync, masterItemsList.ToArray(), originatingRequestID };
        return SendNetMessage(data);
    }

    private void SyncInventoryResponse(Item[] newItems, int originalRequestID) {
        //No matter what, we need to know what has changed from now to then??
        List<Item> removedItems = new List<Item>();
        removedItems.AddRange(items);//Add the old set of items
        removedItems.Union(newItems);//Union with the new set of items.
        List<Item> addedItems = new List<Item>();
        addedItems.AddRange(removedItems);//Rather than taking the union again, just copy the list already created.
        removedItems.Except(newItems);
        addedItems.Except(items);
        items.Clear();
        items.AddRange(newItems);
        //Always invoke this, because even the master will make requests.
        OnItemsUpdated?.Invoke(originalRequestID, addedItems.ToArray(), removedItems.ToArray());
    }

    //This is only run by the master client.
    private void TakeItemRequestReceived(Item[] items, int originalRequestID) {
        foreach (Item i in items) {
            int index = FindItemIndexByNetworkId(i);
            //Ignore requests for items not in this inventory
            if (index != -1) {
                masterItemsList.RemoveAt(index);
            }
        }
        SyncOpenedInventories(originalRequestID);//Syncronize all opened instances of this inventory.
    }

    //This is only run by the master client.
    private void AddItemsRequestReceived ( Item[] items, int originalRequestID ) {
        //Debug.Log("Adding item(s) " + items.Length);
        foreach (Item i in items) {
            int index = FindItemIndexByNetworkId(i);
            //Ignore requests for items not in this inventory
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
                Item[] items = (Item[]) data[2];
                SyncInventoryResponse(items, (int) data[3]);
                break;
            case TakeItemCode:
                if (PhotonNetwork.IsMasterClient) {
                    Item[] ir = (Item[]) data[2];
                    TakeItemRequestReceived(ir, messageMeta.MessageID);
                }
                break;
            case AddItemCode:
                if (PhotonNetwork.IsMasterClient) {
                    Item[] ia = (Item[]) data[2];
                    AddItemsRequestReceived(ia, messageMeta.MessageID);
                }
                break;
        }
    }

    public string SubcodeToString(int subcode) {
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
                return "Unknow";
        }
    }

    public int FindItemIndexByNetworkId ( Item item ) {
        for (int i = 0; i < items.Count; i++) {
            if (items[i].networkID == item.networkID)
                return i;
        }
        return -1;
    }
}
