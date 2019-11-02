using BaD.Chopknee.Utilities;
using BaD.Modules;
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
        private Button ReturnButton;
        [SerializeField]
#pragma warning disable 0649
        private UIInventoryGrid ItemsList;

        public GameObject equipableContextMenu;
        public GameObject consumableContextMenu;
        public GameObject junkContextMenu;

        public ItemInstance currentSelectedItem;

        private NetInventory LocalPlayerInventory;

        public void Open () {
            Open(MainControl.LocalPlayerData.Inventory);
        }

        public void Open ( NetInventory playerInventory ) {
            LocalPlayerInventory = playerInventory;
            ItemsList.Open(LocalPlayerInventory);
            ReturnButton.onClick.AddListener(Close);
            gameObject.SetActive(true);
            ItemsList.OnItemHighlighted += OnItemHighlighted;
            ItemsList.OnItemClicked += OnItemClicked;
        }

        public void Close () {
            ItemsList.OnItemHighlighted -= OnItemHighlighted;
            ItemsList.OnItemClicked -= OnItemClicked;
            ReturnButton.onClick.RemoveListener(Close);
            gameObject.SetActive(false);
            ItemsList.Close();
        }

        void OnItemHighlighted(UIItemChit item) {
            //For now does nothing.
        }

        void OnItemClicked( UIItemChit item) {
            ItemDetailsComponent.ItemData = item.ItemData;
            currentSelectedItem = item.instance;
            //Figure out what menu to show.
            equipableContextMenu.SetActive(false);
            consumableContextMenu.SetActive(false);
            junkContextMenu.SetActive(false);

            switch (item.ItemData.usageType) {
                case ItemType.UseType.Equipable:
                    equipableContextMenu.SetActive(true);
                    break;
                case ItemType.UseType.Consumable:
                    consumableContextMenu.SetActive(true);
                    break;
                case ItemType.UseType.Junk:
                    junkContextMenu.SetActive(true);
                    break;
            }
        }
    }
}