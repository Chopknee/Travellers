using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {

    private List<ItemCard> cards = new List<ItemCard>();
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

    public Inventory() {
        cards = new List<ItemCard>();
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
            
            if (other != null) {
                other.cards.Remove(item);
                cards.Add(item);
            } else {
                //If the other inventory is null, I am assuming this is a new item being spawned in!!
                cards.Add(item.GetNew());
            }
            return true;
        }
        return false;
    }

    //Puts a list of items into this inventory
    public bool PutItems ( ItemCard[] items, Inventory other ) {
        if (maxSize == 0 || cards.Count + items.Length < maxSize) {
            foreach (ItemCard c in items) {
                if (CanHoldMore) {
                    PutItem(c, other);
                }
            }
            return true;
        }
        return false;
    }

}
