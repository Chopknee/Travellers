using BaD.Chopknee.Utilities;
using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Linq;
using UnityEngine;

namespace BaD.Modules.Networking {
    public class NetworkedInventoryManager: MonoBehaviourPunCallbacks {
        public static NetworkedInventoryManager Instance { get; private set; }

        private int nextNetworkItemId = 0;

        [SerializeField]
#pragma warning disable 0649
        private ItemCard[] NetworkedItems;//All items that can exist in an inventory.
        
        public ItemCard GetItemData(Item i) {//Takes the network item reference, and converts it to item data
            return NetworkedItems[i.itemIndex];
        }

        public Item MakeItemStruct(ItemCard ic) {
            int ind = GetItemCardIndex(ic);
            if (ind != -1) {
                return new Item((short)ind);
            }
            //Yeahahahdlfjksle
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

        private void Awake () {
            Instance = this;
            PhotonPeer.RegisterType(typeof(Item), (byte) 'I', Item.Serialize, Item.DeSerialize);
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
