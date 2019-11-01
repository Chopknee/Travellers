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
        }

        public void Close () {
            ItemsList.OnItemHighlighted -= OnItemHighlighted;
            ReturnButton.onClick.RemoveListener(Close);
            gameObject.SetActive(false);
            ItemsList.Close();
        }

        void OnItemHighlighted(UIItemChit item) {
            ItemDetailsComponent.ItemData = item.ItemData;
        }

        void OnItemClicked( UIItemChit item) {

        }
    }
}