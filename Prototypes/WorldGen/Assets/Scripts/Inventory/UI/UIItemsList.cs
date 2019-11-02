using BaD.Modules.Networking;
using BaD.UI.DumpA;
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

    public bool allowLinkedInventoryOnly = false;
    public NetInventory associatedInventory;

    public List<IUIItemcard> items = new List<IUIItemcard>();

    [Tooltip("If left empty, all items may be dropped here.")]
    public ItemModifier[] GroupsList;
    [Tooltip("Sets if the groups list will blackist a collection, or whitelist them.")]
    public bool blackListMode;//

    public int GoldValue {
        get {
            int gv = 0;
            foreach (ItemType card in NetworkedInventoryManager.Instance.GetItemData(associatedInventory.Items)) {
                gv += card.value;
            }
            return gv;
        }
    }

    public bool TryDropItem(UIDraggable item) {
        //Checking if the dropped item is an item card type.
        if (item is IUIItemcard) {
            //Case the dropped item as an item card.
            IUIItemcard uiItemData = (IUIItemcard) item;
            ItemType itemData = uiItemData.CardData;
            bool passed = blackListMode;//Set up the passed variable for the scenario where the card is not part of a group
            passed = passed && allowLinkedInventoryOnly && associatedInventory == uiItemData.OwnerInventory;
            //If there is no group filter specified, or the card is not part of a group, bypass the following nested loop
            if (GroupsList.Length != 0 && itemData.collections.Length != 0 && !allowLinkedInventoryOnly) {
                //Searching through the specified filter groups and the card's groups
                foreach (ItemModifier checkCollection in GroupsList) {
                    foreach (ItemModifier cardCollection in itemData.collections) {
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
                IUIItemcard uiItem = (IUIItemcard) item;
                items.Add(uiItem);
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
        bool successful = true;
        successful &= !res.HasValue || ( res.HasValue && res.Value );

        if (successful) {
            //Take the item from the inventory obejct.
            IUIItemcard uiItem = (IUIItemcard) item;
            items.Remove(uiItem);
        }

        return successful;
    }

    public void TransferComplete(UIDraggable item) {
        if (item is IUIItemcard) {
            OnItemsChanged?.Invoke(this);
        }
//        Debug.Log("Transfer from shop " + debugName + " complete.");
        OnTransferComplete?.Invoke(item);
    }

    public void Cleanup () {
        List<ItemInstance> cardsToPutBack = new List<ItemInstance>();
        foreach (Transform child in transform) {
            if (child.GetComponent<UIDraggable>() != null) {
                GameObject.Destroy(child.gameObject);
                if (child.GetComponent<IUIItemcard>() != null) {
                    cardsToPutBack.Add(child.GetComponent<IUIItemcard>().sourceItem);
                }
            }
        }
        associatedInventory.AddItems(cardsToPutBack.ToArray());
        items.Clear();
    }
}

