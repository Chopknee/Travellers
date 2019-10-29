using System.Collections.Generic;
using UnityEngine;
using BaD.Modules.Networking;
using UnityEngine.UI;
using BaD.UI.DumpA;
using BaD.Chopknee.Utilities;

public class UIInventoryGrid: MonoBehaviour {

    [HideInInspector]
    public NetInventory associatedInventory;

    [SerializeField]
#pragma warning disable 0649
    private GameObject ItemWidgetPrefab;

    public ScrollRect ItemsList;

    public delegate void ItemClicked ( UIItemChit item );
    public ItemClicked OnItemClicked;
    public delegate void ItemsChanged ( UIInventoryGrid caller );
    public ItemsChanged OnItemsChanged;//Run when the items in this inventory list change.
    public delegate void ItemHighlighted ( UIItemChit item );
    public ItemHighlighted OnItemHighlighted;

    void Start () {
        ItemsList = GetComponentInChildren<ScrollRect>();
    }

    public void Open(NetInventory associatedInventory) {
        this.associatedInventory = associatedInventory;
        this.associatedInventory.OnItemsUpdated += OnInventorySynced;
        this.associatedInventory.Open();
    }

    public void Close() {
        associatedInventory.OnItemsUpdated -= OnInventorySynced;
        associatedInventory.Close();
    }

    public void OnInventorySynced ( int originalRequestID, ItemInstance[] addedItems, ItemInstance[] removedItems ) {
        //Adding and removing specific items, rather than whole cloth clearing the list.
        Debug.Log("Added " + addedItems.Length + " Removed " + removedItems.Length);
        foreach (ItemInstance removed in removedItems) {
            int ind = 0;
            foreach (ItemInstance item in associatedInventory.Items) {
                if (item == removed) {
                    Destroy(ItemsList.content.GetChild(ind));
                }
                ind++;
            }
        }

        foreach (ItemInstance added in addedItems) {
            GameObject wid = MakeItemWidget(added);
            wid.transform.SetParent(ItemsList.content);
        }

        OnItemsChanged?.Invoke(this);

        //if (added.Length != 0 || removed.Length != 0) {
        //    ClearItems();
        //    ItemInstance[] theItems = associatedInventory.Items;
        //    foreach (ItemInstance item in theItems) {
        //        GameObject wid = MakeItemWidget(item);
        //        wid.transform.SetParent(ItemsList.content);
        //    }
        //    OnItemsChanged?.Invoke(this);
        //}
    }

    //Destroys all item objects currently in the inventory gui
    public void ClearItems () {
        List<GameObject> dest = new List<GameObject>();
        for (int i = 0; i < ItemsList.content.childCount; i++) {
            dest.Add(ItemsList.content.GetChild(i).gameObject);
            dest[i].GetComponent<UIItemChit>().OnClicked -= ChitClicked;
            dest[i].GetComponent<UIItemChit>().OnHighlighted -= ChitHighlighted;
        }
        Choptilities.DestroyList(dest);
    }

    //Generates a gameobject instance based on the item data
    GameObject MakeItemWidget ( ItemInstance data ) {
        ItemType item = data.details;
        GameObject go = Instantiate(ItemWidgetPrefab);
        UIItemChit uiic = go.GetComponent<UIItemChit>();
        uiic.instance = data;
        uiic.ItemData = item;
        uiic.OnClicked += ChitClicked;
        uiic.OnHighlighted += ChitHighlighted;
        return go;
    }

    //Gets the next child
    public GameObject GetNextChild( UIItemChit currentSelected ) {
        //Returns null if no child is set.
        for (int i = 0; i < ItemsList.content.childCount; i++) {
            if (ItemsList.content.GetChild(i) == currentSelected.gameObject) {
                //Get the next one up
                if (i+1 < ItemsList.content.childCount) {
                    return ItemsList.content.GetChild(i + 1).gameObject;
                }
            }
        }
        return null;
    }

    public GameObject GetFirstChild() {
        if (ItemsList.content.childCount > 0) {
            return ItemsList.content.GetChild(0).gameObject;
        }
        return null;
    }

    void ChitClicked ( UIItemChit clickedItem ) {
        OnItemClicked?.Invoke(clickedItem);
    }

    void ChitHighlighted(UIItemChit highlightedItem ) {
        OnItemHighlighted?.Invoke(highlightedItem);
    }

}
