using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITradeSide : MonoBehaviour, IUIDropZone {

    public string debugName = "";

    public delegate bool DropItem(UIDraggable item);
    public DropItem OnDropItem;
    public delegate bool GrabItem(UIDraggable item);
    public GrabItem OnGrabItem;
    public delegate void TxComplete(UIDraggable item);
    public TxComplete OnTransferComplete;

    public List<ItemCard> containedCards;

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
        Debug.Log("Item received in shop " + debugName );
        bool? res = OnDropItem?.Invoke(item);
        bool result = true;
        if (res.HasValue) {
            result = res.Value;
        }

        return true;
    }

    public bool TryGrabItem(UIDraggable item) {
        Debug.Log("Item taken from shop " + debugName);
        bool? res = OnGrabItem?.Invoke(item);
        if (res.HasValue) {
            return res.Value;
        }
        return true;
    }

    public void TransferComplete(UIDraggable item) {
        Debug.Log("Transfer from shop " + debugName + " complete.");
        OnTransferComplete?.Invoke(item);
    }
}

