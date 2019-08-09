using BaD.Chopknee.Utilities;
using BaD.Modules.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaD.UI.DumpA {
    public class UIPlayerInventory: MonoBehaviour {

        [SerializeField]
#pragma warning disable 0649
        private UIItemInfoObject ItemDetailsComponent;
        [SerializeField]
#pragma warning disable 0649
        private GameObject ItemWidgetPrefab;
        [SerializeField]
#pragma warning disable 0649
        private Button ReturnButton;
        [SerializeField]
#pragma warning disable 0649
        private ScrollRect ItemsList;

        private NetInventory LocalPlayerInventory;

        public void Open ( NetInventory playerInventory ) {
            //Yay, more network based garbage
            LocalPlayerInventory = playerInventory;
            LocalPlayerInventory.OnItemsUpdated += OnInventorySynced;
            gameObject.SetActive(true);
            ReturnButton.onClick.AddListener(Close);
            LocalPlayerInventory.Open();
        }

        public void Close () {
            ReturnButton.onClick.RemoveListener(Close);
            ClearItems();
            LocalPlayerInventory.OnItemsUpdated -= OnInventorySynced;
            gameObject.SetActive(false);
        }

        public void OnInventorySynced ( int originalRequestID, Item[] added, Item[] removed ) {
            Debug.Log("Filling player inventory!");
            //Clear out the old children of the list.
            ClearItems();
            //Make new children to add in the list.
            Item[] theItems = LocalPlayerInventory.Items;
            foreach (Item item in theItems) {
                GameObject wid = MakeItemWidget(item);
                wid.transform.SetParent(ItemsList.content);
            }
            //ItemsList.content.chil
        }

        public void ClearItems() {
            List<GameObject> dest = new List<GameObject>();
            for (int i = 0; i < ItemsList.content.childCount; i++) {
                dest.Add(ItemsList.content.GetChild(i).gameObject);
            }
            Choptilities.DestroyList(dest);
        }

        GameObject MakeItemWidget ( Item data ) {
            ItemCard item = NetworkedInventoryManager.Instance.GetItemData(data);
            GameObject go = Instantiate(ItemWidgetPrefab);
            UIItemChit uiic = go.GetComponent<UIItemChit>();
            uiic.ItemData = item;
            uiic.OnClicked += ChitClicked;
            return go;
        }

        void ChitClicked(UIItemChit oneDatDunGotClicked) {
            //We know which one got clicked!!
            ItemDetailsComponent.ItemData = oneDatDunGotClicked.ItemData;
        }
    }
}