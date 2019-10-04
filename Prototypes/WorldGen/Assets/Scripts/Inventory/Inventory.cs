using BaD.Modules.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaD.Modules.Inventory {
    public class Inventory: MonoBehaviour {

        public ItemInstance[] Items {
            get {
                return items.ToArray();
            }
        }

        List<ItemInstance> items = new List<ItemInstance>();

        public void AddItem ( ItemInstance item ) {
            items.Add(item);
        }

        public void AddItems ( ItemInstance[] items ) {
            this.items.AddRange(items);
        }

        public void RemoveItem ( ItemInstance item ) {
            items.Remove(item);
        }

        public void RemoveItems ( ItemInstance[] items ) {
            foreach (ItemInstance i in items) {
                this.items.Remove(i);
            }
        }

        public void Clear () {
            items.Clear();
        }

        //Yay, so much confusion
        public static ItemInstance[] GetItemsByGroup ( ItemInstance[] items, Collection[] filters ) {
            List<ItemInstance> filtered = new List<ItemInstance>();
            foreach (Collection coll in filters) {
                foreach (ItemInstance i in items) {
                    Collection[] cardCollections = NetworkedInventoryManager.Instance.GetItemData(i).collections;
                    foreach (Collection itemCollection in cardCollections) {
                        if (itemCollection == coll && !filtered.Contains(i)) {
                            filtered.Add(i);
                            break;
                        }
                    }
                }
            }
            return filtered.ToArray();
        }
    }
}