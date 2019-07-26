using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryList : MonoBehaviour, IUIDropZone {

    public string debugName = "";
    [Tooltip("If left empty, all items may be dropped here.")]
    public Collection[] GroupsList;
    [Tooltip("Sets if the groups list will blackist a collection, or whitelist them.")]
    public bool blackListMode;//

    public bool TryDropItem(UIDraggable item) {
        if (item is IUIItemcard) {
            //Cast the item card as an IUIItemCard???
            ItemCard data = ((IUIItemcard)item).CardData;//Does this actually work!?!?!?
            //Debug.Log("A " + data.name + " was dropped in " + debugName);
            if (GroupsList.Length == 0) {return true;}//If the group list is empty, the check automatically passes.
            if (data.collections.Length == 0) {return blackListMode;}//If the card is not part of a group, only pass it if we are in blacklist mode???
            bool passed = blackListMode;
            foreach (Collection checkCollection in GroupsList) {
                foreach (Collection cardCollection in data.collections) {
                    if (blackListMode) {
                        //Whitelist mode if any condition is true
                        passed = passed && checkCollection != cardCollection;
                    } else {
                        //Blacklist mode, if all conditions are true
                        passed = passed || checkCollection == cardCollection;
                    }
                }
            }
            return passed;
        }
        //Debug.Log("An object not inheriting from IUIItemCard was dropped in " + debugName );
        return false;
    }

    public bool TryGrabItem(UIDraggable item) {
        //Debug.Log("Item taken from shop " + debugName);
        return true;
    }

    public void TransferComplete(UIDraggable item) {
        //Do nothing!
        //Debug.Log("Transfer from shop " + debugName + " complete.");
    }
}
