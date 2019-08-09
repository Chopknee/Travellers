/*
    The base for all draggable card object types.
        Since I don't know what the cards should look like (if they even will be cards)
        this class acts as an intermediary between the ui draggable and the way the card is displayed.
 */
using BaD.Modules.Networking;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BaD.UI.DumpA {
    public abstract class IUIItemcard: UIDraggable {
        [HideInInspector]
        public Item sourceItem;
        [HideInInspector]
        public NetInventory OwnerInventory;

        public ItemCard CardData {
            get {
                return cd;
            }
            set {
                CardDataChanged(value);
                cd = value;
            }
        }

        private ItemCard cd;

        public override void OnDrop ( PointerEventData eventData ) {
            base.OnDrop(eventData);
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

            if (dropzone == null) {
                //Let the owning inventory know that this needs to be put back
                OwnerInventory.AddItem(sourceItem);
            }
        }

        //This function should be overriden to update the card gui
        public abstract void CardDataChanged ( ItemCard newCard );
    }
}