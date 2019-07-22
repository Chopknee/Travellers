using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICard : MonoBehaviour, IDragHandler, IDropHandler, IBeginDragHandler {

    public delegate void PickedUp ();

    public delegate bool Dropped (Inventory from, Inventory to);
    public Dropped OnDropped;

    Transform originalParent;
    Inventory originatingInventory;//The originating inventory

    public void OnDrag ( PointerEventData eventData ) {
        //This will continue to move the card with the mouse.
        transform.position = Input.mousePosition;
        //Should probably use the drag start handler for this.
    }

    public void OnDrop ( PointerEventData eventData ) {
        //Force refresh the pointer because it doesn't properly refresh here?
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        GameObject droppedSlot = null;
        foreach (RaycastResult rr in results) {
            GameObject go = rr.gameObject;
            if (go.GetComponent<UIInventorySlot>() != null) {
                droppedSlot = go;
                //break;
            }
        }

        if (droppedSlot != null && droppedSlot.GetComponentInParent<Inventory>()) {
            //Reverse the drag back to original postion;
            Inventory newInv = droppedSlot.GetComponentInParent<Inventory>();
            if (newInv.PutItem(GetComponent<ItemCardObject>().CardData, originatingInventory)) {
                originalParent = droppedSlot.transform;
                originatingInventory = droppedSlot.GetComponentInParent<Inventory>();
            }
        }

        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;
    }

    public void OnBeginDrag ( PointerEventData eventData ) {
        originalParent = transform.parent;
        transform.SetParent(CardDragger.Instance.transform);
        originatingInventory = originalParent.GetComponentInParent<Inventory>();
    }
}
