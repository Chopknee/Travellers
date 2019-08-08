using BaD.Modules.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public Item[] Items {
        get {
            return items.ToArray();
        }
    }

    List<Item> items = new List<Item>();

    public void AddItem(Item item) {
        items.Add(item);
    }

    public void AddItems(Item[] items) {
        this.items.AddRange(items);
    }

    public void RemoveItem(Item item) {
        items.Remove(item);
    }

    public void RemoveItems(Item[] items) {
        foreach (Item i in items) {
            this.items.Remove(i);
        }
    }

    public void Clear() {
        items.Clear();
    }
}
