using BaD.Chopknee.Utilities;
using System;
using System.Collections.Generic;

namespace BaD.Modules.Networking {

    [Serializable]
    public class ItemInstance: IEquatable<ItemInstance> {

        //This is associated with an item in the NetworkedItems array of NetworkedInventoryManager
        public short itemIndex { get; }
        //Unique to this specific item instance used for identifying this item as separate from others in the game when instantiated for any reason.
        public short networkID { get; }

        public ItemType details {
            get {
                return NetworkedInventoryManager.Instance.GetItemData(this);
            }
        }

        public ItemInstance ( short itemCardIndex ) {
            itemIndex = itemCardIndex;
            networkID = 0;
            if (NetworkedInventoryManager.Instance != null) {
                networkID = (short) NetworkedInventoryManager.Instance.GetItemNetworkId();
            }
        }

        private ItemInstance ( short itemCardIndex, short netId ) {
            itemIndex = itemCardIndex;
            networkID = netId;
        }

        public static byte[] Serialize ( object received ) {
            byte[] arr = new byte[4];
            ItemInstance item = (ItemInstance) received;
            Choptilities.ShortToBytes(item.itemIndex, out arr[0], out arr[1]);
            Choptilities.ShortToBytes(item.networkID, out arr[2], out arr[3]);
            return arr;
        }

        public static object DeSerialize ( byte[] received ) {
            ItemInstance i = new ItemInstance(
                Choptilities.ByteToShort(received[0], received[1]),
                Choptilities.ByteToShort(received[2], received[3])
                );
            return i;
        }

        public bool Equals ( ItemInstance i ) {
            return itemIndex == i.itemIndex && networkID == i.networkID;
        }

        public override int GetHashCode() {
            return networkID.GetHashCode();
        }
    }

    internal class ItemInstanceComparer: IEqualityComparer<ItemInstance> {

        public bool Equals ( ItemInstance x, ItemInstance y ) {
            return x.itemIndex == y.itemIndex && x.networkID == y.networkID;
        }

        public int GetHashCode ( ItemInstance obj ) {
            return obj.networkID.GetHashCode();
        }
    }
}
