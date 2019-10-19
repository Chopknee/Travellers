using BaD.Chopknee.Utilities;
using BaD.Modules;
using BaD.Modules.Networking;
using BaD.UI.DumpA;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopTrade : MonoBehaviour {

    private PlayerData playerData;
    private NetInventory shopInventory;
    [SerializeField]
#pragma warning disable 0649
    private UIInventory playerInventoryPanel;
    [SerializeField]
#pragma warning disable 0649
    private UIInventory shopInventoryPanel;
    [SerializeField]
#pragma warning disable 0649
    private UIInventory sellWindow;
    [SerializeField]
#pragma warning disable 0649
    private UIInventory buyWindow;
    [SerializeField]
#pragma warning disable 0649
    private Button btnExecuteTrade;
    [SerializeField]
#pragma warning disable 0649
    private Button btnExitTrade;
    [SerializeField]
#pragma warning disable 0649
    private Text txtSellValue;
    [SerializeField]
#pragma warning disable 0649
    private Text txtBuyValue;
    [SerializeField]
#pragma warning disable 0649
    private Text txtDirections;
    [SerializeField]
#pragma warning disable 0649
    private Text txtPlayerGold;
    [SerializeField]
#pragma warning disable 0649
    private Text txtNet;
    [SerializeField]
#pragma warning disable 0649
    private Text shopName;
    [SerializeField]
#pragma warning disable 0649
    private Text playerName;

    public delegate void Closed();
    public Closed OnClosed;

    public void Awake() {
        gameObject.SetActive(false);
    }

    public void ShowTradeWindow ( string shopName, NetInventory shopInventory, PlayerData playerToTradeWith ) {

        if (gameObject.activeSelf) { return; }
        if (playerToTradeWith == null) { Debug.Log("The trading player was null."); }
        if (playerToTradeWith.Inventory == null) { Debug.Log("The trading player's inventory was null."); }

        gameObject.SetActive(true);
        btnExecuteTrade.onClick.AddListener(ExecuteTrade);
        sellWindow.OnItemsChanged += OnItemsListChanged;
        buyWindow.OnItemsChanged += OnItemsListChanged;
        btnExitTrade.onClick.AddListener(ExitTrade);
        btnExecuteTrade.interactable = false;
        this.shopInventory = shopInventory;
        playerData = playerToTradeWith;
        this.shopName.text = shopName;
        playerName.text = playerToTradeWith.Name;
        txtPlayerGold.text = "+ On Hand: " + string.Format("{0, 0:D3}g", playerData.gold);

        //Enables the network interface for the inventory and whatnot.
        shopInventoryPanel.Open(shopInventory);
        playerInventoryPanel.Open(playerToTradeWith.Inventory);

        //Simply creates an instance of inventory for the local windows.
        sellWindow.Open(playerToTradeWith.Inventory);
        buyWindow.Open(shopInventory);
        MainControl.Instance.SetPlayerControl(false);
    }

    public void OnItemsListChanged(UIInventory caller ) {
        //huh
        //Debug.Log("Items list has changed!!!");
        txtSellValue.text = "+ Sell: " + string.Format("{0, 0:D3}g", sellWindow.GoldValue);
        txtBuyValue.text = "- Buy: " + string.Format("{0, 0:D3}g", buyWindow.GoldValue);
        txtNet.text = "Net: " + string.Format("{0, 0:D3}g", sellWindow.GoldValue - buyWindow.GoldValue + playerData.gold);
        /*
            Rules for the trade to execute successfully
            There has to be items that are being purchased or sold.
            The total value must not be greater than the amount of gold the current player has.

            total value is calculated by taking the value of the sell window and subtracting it from the value of the sell window.

        */
        btnExecuteTrade.interactable = false;
        txtDirections.text = "";
        if (sellWindow.Items.Length != 0 || buyWindow.Items.Length != 0) {
            int value = sellWindow.GoldValue - buyWindow.GoldValue;
            if (value < 0 && playerData.gold >= Mathf.Abs(value) || value >= 0) {
                //Player has enough gold to buy the items.
                btnExecuteTrade.interactable = true;
            } else {
                txtDirections.text = "Not enough gold!";
            }   
        }
    }



    public void ExecuteTrade() {
        //Complete the trade!!!
        txtPlayerGold.text = "+ On Hand: " + string.Format("{0, 0:D3}g", playerData.gold);
        int value = sellWindow.GoldValue - buyWindow.GoldValue;
        playerData.gold += value;

        //The exchange of items between the inventories
        List<ItemInstance> playerPurchasedItems = new List<ItemInstance>();
        foreach (ItemInstance item in buyWindow.Items) {
            playerPurchasedItems.Add(item);
        }
        if (playerPurchasedItems.Count > 0) {
            playerData.Inventory.AddItems(playerPurchasedItems.ToArray());
        }

        List<ItemInstance> playerSoldItems = new List<ItemInstance>();
        foreach (ItemInstance item in sellWindow.Items) {
            playerSoldItems.Add(item);
        }
        if (playerSoldItems.Count > 0) {
            shopInventory.AddItems(playerSoldItems.ToArray());
        }

        sellWindow.Cleanup();
        buyWindow.Cleanup();

        //quick and dirty reset code ;)
        ExitTrade();
        ShowTradeWindow(shopName.text, shopInventory, playerData);
        //Quick and dirty fix ;;;)))
        txtSellValue.text = "+ Sell: " + string.Format("{0, 0:D3}g", sellWindow.GoldValue);
        txtBuyValue.text = "- Buy: " + string.Format("{0, 0:D3}g", buyWindow.GoldValue);
        txtNet.text = "Net: 000g";

    }

    public void ExitTrade() {
        //Revert any unfinished trades
        List<ItemInstance> moveBacks;
        if (sellWindow.Items.Length != 0) {
            //Revert the player items back to their inventory
            moveBacks = new List<ItemInstance>();
            foreach (ItemInstance i in sellWindow.Items) {
                moveBacks.Add(i);
            }
            playerData.Inventory.AddItems(moveBacks.ToArray());
        }
        //sellWindow.Items
        if (buyWindow.Items.Length != 0) {
            //Revert the shop items back to their inventory
            moveBacks = new List<ItemInstance>();
            foreach (ItemInstance i in buyWindow.Items) {
                moveBacks.Add(i);
            }
            shopInventory.AddItems(moveBacks.ToArray());
        }
        //Cleanup of the networked inventory stuff.
        shopInventoryPanel.Close();
        playerInventoryPanel.Close();
        sellWindow.Close();
        sellWindow.Cleanup();
        buyWindow.Close();
        buyWindow.Cleanup();

        //Closes the trade window.
        btnExecuteTrade.onClick.RemoveListener(ExecuteTrade);
        sellWindow.OnItemsChanged -= OnItemsListChanged;
        buyWindow.OnItemsChanged -= OnItemsListChanged;
        btnExitTrade.onClick.RemoveListener(ExitTrade);
        gameObject.SetActive(false);
        OnClosed?.Invoke();
        MainControl.Instance.SetPlayerControl(true);
    }
}
