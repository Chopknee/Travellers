using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Inventory : MonoBehaviour {

    public List<ItemCard> cards = new List<ItemCard>();
    public int ItemCount {
        get {
            return cards.Count;
        }
    }
    public int maxSize;

    public bool CanHoldMore {
        get {
            return maxSize == 0 || cards.Count < maxSize;
        }
    }

    //Gets a copy of all items in this inventory
    public ItemCard[] GetItems () {
        List<ItemCard> newList = new List<ItemCard>();
        newList.AddRange(cards);
        return newList.ToArray();
    }

    //public ItemCard TakeItem (int index) {
    //    ItemCard c = cards[index];
    //    cards.RemoveAt(index);
    //    return c;
    //}

    //Adds an item to this inventory
    public bool PutItem ( ItemCard item, Inventory other) {
        if (CanHoldMore) {
            cards.Add(item);
            other.cards.Remove(item);
            return true;
        }
        return false;
    }

    //Puts a list of items into this inventory
    public bool PutItems ( ItemCard[] items, Inventory other ) {
        if (maxSize == 0 || cards.Count + items.Length < maxSize) {
            foreach (ItemCard c in items) {
                if (CanHoldMore) {
                    cards.Add(c);
                    other.cards.Remove(c);
                }
            }
            return true;
        }
        return false;
    }

}
