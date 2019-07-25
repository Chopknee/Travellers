using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemsList : MonoBehaviour, IUIDropZone {

    public string debugName = "";
    public GameObject itemCardPrefab;

    public delegate bool DropItem(UIDraggable item);
    public DropItem OnDropItem;
    public delegate bool GrabItem(UIDraggable item);
    public GrabItem OnGrabItem;
    public delegate void TxComplete(UIDraggable item);
    public TxComplete OnTransferComplete;

    public delegate void ItemsChanged(UIItemsList caller);
    public ItemsChanged OnItemsChanged;

    public List<ItemCard> containedCards;

    public bool allowLinkedInventoryOnly = false;
    public Inventory associatedInventory;

    [Tooltip("If left empty, all items may be dropped here.")]
    public Collection[] GroupsList;
    [Tooltip("Sets if the groups list will blackist a collection, or whitelist them.")]
    public bool blackListMode;//

    public int GoldValue {
        get {
            int gv = 0;
            foreach (ItemCard card in containedCards) {
                gv += card.value;
            }
            return gv;
        }
    }

    public bool TryDropItem(UIDraggable item) {
        //Checking if the dropped item is an item card type.
        if (item is IUIItemcard) {
            //Case the dropped item as an item card.
            ItemCard cardData = ((IUIItemcard)item).CardData;
            bool passed = blackListMode;//Set up the passed variable for the scenario where the card is not part of a group
            passed = passed && allowLinkedInventoryOnly && associatedInventory == cardData.owningInventory;
            //If there is no group filter specified, or the card is not part of a group, bypass the following nested loop
            if (GroupsList.Length != 0 && cardData.collections.Length != 0 && !allowLinkedInventoryOnly) {
                //Searching through the specified filter groups and the card's groups
                foreach (Collection checkCollection in GroupsList) {
                    foreach (Collection cardCollection in cardData.collections) {
                        if (blackListMode) {
                            //Whitelist mode if any condition is true
                            passed = passed && checkCollection != cardCollection;
                        } else {
                            //Blacklist mode, if all conditions are true
                            passed = passed || checkCollection == cardCollection;
                        }
                    }
                }
            }
            //Attempt to run any hooked-in functions
            bool? res = OnDropItem?.Invoke(item);
            passed = passed && (!res.HasValue || res.HasValue && res.Value);
            //If passed is true at this point, then we can transfer the card!!
            if (passed) {
                ItemCard c = ((IUIItemcard)item).CardData;
                containedCards.Add(c);
                OnItemsChanged?.Invoke(this);
            }
            return passed;

        }
        Debug.Log("An object not inheriting from IUIItemCard was dropped in " + debugName );
        return false;
    }

    public bool TryGrabItem(UIDraggable item) {
        //Debug.Log("Item taken from shop " + debugName);
        bool? res = OnGrabItem?.Invoke(item);
        if (res.HasValue) {
            return res.Value;
        }
        return true;
    }

    public void TransferComplete(UIDraggable item) {
        if (item is IUIItemcard) {
            ItemCard c = ((IUIItemcard)item).CardData;
            containedCards.Remove(c);
            OnItemsChanged?.Invoke(this);
        }
//        Debug.Log("Transfer from shop " + debugName + " complete.");
        OnTransferComplete?.Invoke(item);
    }

    public void InstantiateItem(ItemCard cardData) {
        GameObject go = Instantiate(itemCardPrefab);
        go.GetComponent<ItemWidget>().CardData = cardData;
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        containedCards.Add(cardData);
    }

    public void Cleanup() {
        foreach (Transform child in transform) {
            if (child.GetComponent<UIDraggable>() != null) {
                GameObject.Destroy(child.gameObject);
            }
        }
        containedCards.Clear();
    }
}

