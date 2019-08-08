using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDraggable : MonoBehaviour, IDragHandler, IDropHandler, IBeginDragHandler {

    public Transform originalParent;

    public Vector3 originalPosition = Vector3.zero;

    private bool dragAllowed = false;

    public bool Dragging {
        get {
            return dragging;
        }
    }
    private bool dragging;

    public void Start() {
        originalParent = transform.parent;
    }

    public void OnDrag ( PointerEventData eventData ) {
        if (dragAllowed) {
            //This will continue to move the card with the mouse.
            transform.position = Input.mousePosition;
            //Should probably use the drag start handler for this.
        }
    }

    public virtual void OnDrop ( PointerEventData eventData ) {
        dragging = false;
        //Force refresh the pointer because it doesn't properly refresh here?
        eventData.position = Input.mousePosition;//Refreshing pointer position.
        List<RaycastResult> results = new List<RaycastResult>();//List that stores raycast results
        EventSystem.current.RaycastAll(eventData, results);//Run a raycast to find ui elements under the cursor
        GameObject dropzone = null;
        foreach (RaycastResult rr in results) {//Loop through each object found
            GameObject go = rr.gameObject;
            if (go.GetComponent<IUIDropZone>() != null) {//Looking specifically for the drop zone object.
                dropzone = go;
                break;
            }
        }

        if (dropzone != null && dropzone.GetComponent<IUIDropZone>().TryDropItem(this)) {
            if (originalParent != null && originalParent.gameObject.GetComponent<IUIDropZone>() != null) {
                originalParent.gameObject.GetComponent<IUIDropZone>().TransferComplete(this);
            }
            transform.SetParent(dropzone.transform);
            originalParent = dropzone.transform;
        } else {
            transform.SetParent(originalParent);
            transform.position = originalPosition;
        }
    }

    public void OnBeginDrag ( PointerEventData eventData ) {
        if (originalParent != null) {
            if (originalParent.gameObject.GetComponent<IUIDropZone>() != null && originalParent.gameObject.GetComponent<IUIDropZone>().TryGrabItem(this)) {
                dragAllowed = true;
                originalParent = transform.parent;
                originalPosition = transform.position;
                transform.SetParent(gameObject.GetComponentInParent<Canvas>().transform);
                dragging = true;
                return;
            }
        }
        dragAllowed = false;
    }
}
