using BaD.Chopknee.Utilities;
using BaD.Modules.Inventory;
using BaD.Modules.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaD.UI.DumpA {

    public class UIInventory: MonoBehaviour, IUIDropZone {
        //A few delegates to hook into

        public delegate bool DropItem ( UIDraggable item );
        public DropItem OnDropItem;//Run when an item is dropped on this inventory.
        public delegate bool GrabItem ( UIDraggable item );
        public GrabItem OnGrabItem;//Run when an item is taken from this inventory
        public delegate void TxComplete ( UIDraggable item );
        public TxComplete OnTransferComplete;//Run when a transfer is completed???
        public delegate void ItemsChanged ( UIInventory caller );
        public ItemsChanged OnItemsChanged;//Run when the items in this inventory list change.

        [SerializeField]
        [Tooltip("The object that items spawn as.")]
#pragma warning disable 0649
        private GameObject itemWidgetPrefab;
        [Tooltip("A name used for debugging purposes.")]
        public string debugName = "";
        [Tooltip("Only allows transfers from the linked inventory window.")]
        public bool allowLinkedInventoryOnly = false;
        [SerializeField]
        [Tooltip("If allow linked inventory only is set, items will only be accepted from this inventory.")]
#pragma warning disable 0649
        private UIInventory linkedInventoryWindow;
        [Tooltip("If left empty, all items may be dropped here.")]
        public ItemModifier[] GroupsList;
        [Tooltip("Sets if the groups list will blackist a collection, or whitelist them.")]
        public bool blackListMode;//
        [SerializeField]
        [Tooltip("Set this if the items in this inventory should only be visible to the local player.")]
#pragma warning disable 0649
        private bool UseLocalInventory;


        private List<GameObject> ownedItemInstances = new List<GameObject>();

        public NetInventory networkedInventory;
        public Inventory localInventory;
        
        public int LastInventoryRequestID = 0;

        public void Open ( NetInventory inv ) {

            networkedInventory = inv;
            if (!UseLocalInventory) {
                networkedInventory.OnItemsUpdated += OnInventorySynced;
                networkedInventory.Open();
            } else {
                if (localInventory == null) {
                    if (GetComponent<Inventory>() == null) {
                        gameObject.AddComponent<Inventory>();
                    }
                    localInventory = GetComponent<Inventory>();
                }
            }
        }

        public void Close () {
            Choptilities.DestroyList(ownedItemInstances);
            ownedItemInstances.Clear();
            if (!UseLocalInventory) {
                networkedInventory.OnItemsUpdated -= OnInventorySynced;
                networkedInventory.Close();
            }
        }

        public void Cleanup() {
            localInventory.Clear();
            Choptilities.DestroyList(ownedItemInstances);
            ownedItemInstances.Clear();
        }

        public int GoldValue {
            get {
                int gv = 0;
                if (UseLocalInventory) {
                    foreach (ItemType card in NetworkedInventoryManager.Instance.GetItemData(localInventory.Items)) {
                        gv += card.value;
                    }
                } else {
                    foreach (ItemType card in NetworkedInventoryManager.Instance.GetItemData(networkedInventory.Items)) {
                        gv += card.value;
                    }
                }
                return gv;
            }
        }

        public ItemInstance[] Items {
            get {
                if (UseLocalInventory) {
                    return localInventory.Items;
                } else {
                    return networkedInventory.Items;
                }
            }
        }

        public bool TryDropItem ( UIDraggable item ) {
            //Checking if the dropped item is an item card type.
            if (item is IUIItemcard) {
                //Case the dropped item as an item card.
                IUIItemcard uiItemData = (IUIItemcard) item;
                UIInventory otherInventory = uiItemData.originalParent.GetComponent<UIInventory>();
                ItemType itemData = uiItemData.CardData;
                bool passed = blackListMode;//Set up the passed variable for the scenario where the card is not part of a group
                passed = passed && allowLinkedInventoryOnly && otherInventory.linkedInventoryWindow == this;
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
                passed = passed && ( !res.HasValue || res.HasValue && res.Value );
                //If passed is true at this point, then we can transfer the card!!
                if (passed) {
                    if (UseLocalInventory) {
                        localInventory.AddItem(uiItemData.sourceItem);
                    } else {
                        networkedInventory.AddItem(uiItemData.sourceItem);
                    }
                    //This may possibly work??
                    otherInventory.ownedItemInstances.Remove(uiItemData.gameObject);
                    ownedItemInstances.Add(uiItemData.gameObject);
                    OnItemsChanged?.Invoke(this);
                } else {
                    //The drop was unsuccessful, add the item back to it's originating inventory.
                    IUIItemcard uiItem = (IUIItemcard) item;
                    uiItem.OwnerInventory.AddItem(uiItem.sourceItem);
                }
                return passed;
            }
            return false;
        }

        public bool TryGrabItem ( UIDraggable item ) {
            bool? res = OnGrabItem?.Invoke(item);
            bool successful = true;
            successful &= !res.HasValue || ( res.HasValue && res.Value );

            if (successful) {
                IUIItemcard ic = item.GetComponent<IUIItemcard>();
                ic.safe = false;
                //Take the item from the inventory obejct.
                if (UseLocalInventory) {
                    localInventory.RemoveItem(ic.sourceItem);
                } else {
                    LastInventoryRequestID = networkedInventory.RemoveItem(ic.sourceItem);
                }
            }

            return successful;
        }

        public void TransferComplete ( UIDraggable item ) {
            if (item is IUIItemcard) {
                OnItemsChanged?.Invoke(this);
            }
            OnTransferComplete?.Invoke(item);
        }

        //This is executed by the network inventory object when it receives inventory sync requests for this specific inventory.
        public void OnInventorySynced ( int originalRequestID, ItemInstance[] added, ItemInstance[] removed ) {
            //Debug.Log("Received an inventory sync with id of " + originalRequestID + " Inventory size: " + associatedInventory.Items.Length);
            if (LastInventoryRequestID == originalRequestID) {
                List<GameObject> keepers = new List<GameObject>();
                foreach (GameObject go in  ownedItemInstances) {
                    if (go != null && !go.GetComponent<IUIItemcard>().Dragging) {
                        Destroy(go);//Yeh?
                    } else {
                        keepers.Add(go);
                        go.GetComponent<IUIItemcard>().safe = true;
                    }
                }
                ownedItemInstances.Clear();
                ownedItemInstances.AddRange(keepers);
            } else {
                List<GameObject> deathList = new List<GameObject>();
                List<GameObject> keepers = new List<GameObject>();
                foreach (GameObject go in ownedItemInstances) {
                    if (!go.GetComponent<IUIItemcard>().safe) {
                        deathList.Add(go);
                    } else {
                        keepers.Add(go);
                    }
                }
                //There was an update of items, make sure our current is equivalent to the current master
                Choptilities.DestroyList(deathList);//Even if the player is actively dragging an item, it will be killed!!!!! >:)
                ownedItemInstances.Clear();
                ownedItemInstances.AddRange(keepers);
            }


            //This just adds all items in the inventory as items, completely ignoring all else.
            ItemInstance[] shopItems = networkedInventory.Items;
            for (int i = 0; i < shopItems.Length; i++) {
                GameObject go = MakeItemWidget(shopItems[i], networkedInventory);
                go.transform.SetParent(transform);
                ownedItemInstances.Add(go);
            }
        }

        public GameObject MakeItemWidget ( ItemInstance data, NetInventory owner ) {
            ItemType item = NetworkedInventoryManager.Instance.GetItemData(data);
            GameObject go = Instantiate(itemWidgetPrefab);
            IUIItemcard uiic = go.GetComponent<IUIItemcard>();
            uiic.CardData = item;
            uiic.sourceItem = data;
            uiic.OwnerInventory = owner;
            return go;
        }
    }
}