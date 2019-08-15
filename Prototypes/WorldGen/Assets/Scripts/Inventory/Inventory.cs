using BaD.Modules.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaD.Modules.Inventory {
    public class Inventory: MonoBehaviour {

        public Item[] Items {
            get {
                return items.ToArray();
            }
        }

        List<Item> items = new List<Item>();

        public void AddItem ( Item item ) {
            items.Add(item);
        }

        public void AddItems ( Item[] items ) {
            this.items.AddRange(items);
        }

        public void RemoveItem ( Item item ) {
            items.Remove(item);
        }

        public void RemoveItems ( Item[] items ) {
            foreach (Item i in items) {
                this.items.Remove(i);
            }
        }

        public void Clear () {
            items.Clear();
        }

        //Yay, so much confusion
        public static Item[] GetItemsByGroup ( Item[] items, Collection[] filters ) {
            List<Item> filtered = new List<Item>();
            foreach (Collection coll in filters) {
                foreach (Item i in items) {
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