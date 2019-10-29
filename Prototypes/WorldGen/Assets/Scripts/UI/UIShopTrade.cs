using BaD.Chopknee.Utilities;
using BaD.Modules;
using BaD.Modules.Networking;
using BaD.UI.DumpA;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopTrade : MonoBehaviour {

    private PlayerData playerData;
    private NetInventory shopInventory;

    public UIInventoryGrid playerInventoryWindow;
    public UIInventoryGrid shopInventoryWindow;

    public Image arrowImage;
    public Sprite buySprite;
    public Sprite sellSprite;

    public Button exitButton;

    public UIItemInfo itemDescriptionBox;

    public delegate void Closed();
    public Closed OnClosed;

    public void ShowTradeWindow ( string shopName, NetInventory shopInventory, PlayerData playerToTradeWith ) {
        playerData = playerToTradeWith;
        playerInventoryWindow.Open(playerData.Inventory);
        playerInventoryWindow.OnItemsChanged += OnItemsListChanged;//Runs whenever the items get changed (Not that it needs to happen I suppose).
        playerInventoryWindow.OnItemClicked += PlayerItemClicked;
        playerInventoryWindow.OnItemHighlighted += PlayerItemHighlighted;
        this.shopInventory = shopInventory;

        shopInventoryWindow.Open(this.shopInventory);
        shopInventoryWindow.OnItemsChanged += OnItemsListChanged;
        shopInventoryWindow.OnItemClicked += ShopItemClicked;
        shopInventoryWindow.OnItemHighlighted += ShopItemHighlighted;
        exitButton.onClick.AddListener(ExitTrade);
    }

    public void OnItemsListChanged(UIInventoryGrid caller ) {
        Debug.LogFormat("Items refreshed on {0} inventory window.", caller.name);
    }

    public void PlayerItemClicked ( UIItemChit item ) {
        playerData.gold += item.ItemData.value;
        int reqId = playerData.Inventory.RemoveItem(item.instance, PlayerItemRemovedResponse);
        //Select the next possible item 
        GameObject nextChild = playerInventoryWindow.GetNextChild(item);
        if (nextChild != null) {
            EventSystem.current.SetSelectedGameObject(nextChild);
        } else {
            if (shopInventoryWindow.GetFirstChild() != null) {
                EventSystem.current.SetSelectedGameObject(shopInventoryWindow.GetFirstChild());
            } else {
                EventSystem.current.SetSelectedGameObject(exitButton.gameObject);
            }
        }

    }

    public void PlayerItemRemovedResponse( int ogRequestId, bool itemsTaken, bool success, ItemInstance[] items) {
        int gv = 0;
        foreach (ItemInstance i in items) {
            gv += i.details.value;
        }
        if (itemsTaken && success) {
            shopInventory.AddItems(items);
        } else if (itemsTaken && !success) {
            //Did not succeed, must refund.
            playerData.gold -= gv;
        }
    }

    public void ShopItemClicked( UIItemChit item) {
        //Check if the player can afford the item
        if (item.ItemData.value <= playerData.gold) {
            //Set up the request and callback data
            playerData.gold -= item.ItemData.value;//Take the gold now to prevent a player from buying more than they can afford
            int reqId = shopInventory.RemoveItem(item.instance, ShopItemsRemovedResponse);//Send the request

            GameObject nextChild = shopInventoryWindow.GetNextChild(item);
            if (nextChild != null) {
                EventSystem.current.SetSelectedGameObject(nextChild);
            } else {
                if (playerInventoryWindow.GetFirstChild() != null) {
                    EventSystem.current.SetSelectedGameObject(playerInventoryWindow.GetFirstChild());
                } else {
                    EventSystem.current.SetSelectedGameObject(exitButton.gameObject);
                }
            }
        }
    }

    public void ShopItemsRemovedResponse ( int ogRequestId, bool itemsTaken, bool success, ItemInstance[] items ) {
        int gv = 0;
        foreach (ItemInstance i in items) {
            gv += i.details.value;
        }
        if (itemsTaken && success) {
            playerData.Inventory.AddItems(items);
        } else if (itemsTaken && !success) {
            //Did not succeed, must refund.
            playerData.gold += gv;
        }
    }

    public void ShopItemHighlighted( UIItemChit item ) {
        //Debug.Log("Shop item highlighted!");
        //Make the arrow point right
        arrowImage.sprite = buySprite;

        itemDescriptionBox.item = item.instance;

    }

    public void PlayerItemHighlighted( UIItemChit item ) {
        //Debug.Log("Player item highlighted!");

        //Make the arrow point left.
        arrowImage.sprite = sellSprite;

        itemDescriptionBox.item = item.instance;
    }

    public void ExitTrade () {
        Debug.Log("Trade closed.");
        playerInventoryWindow.Close();
        playerInventoryWindow.OnItemsChanged -= OnItemsListChanged;
        playerInventoryWindow.OnItemClicked -= PlayerItemClicked;
        playerInventoryWindow.OnItemHighlighted -= PlayerItemHighlighted;

        shopInventoryWindow.Close();
        shopInventoryWindow.OnItemsChanged -= OnItemsListChanged;
        shopInventoryWindow.OnItemClicked -= ShopItemClicked;
        shopInventoryWindow.OnItemHighlighted -= ShopItemHighlighted;

        exitButton.onClick.RemoveListener(ExitTrade);
        OnClosed?.Invoke();
    }
}
