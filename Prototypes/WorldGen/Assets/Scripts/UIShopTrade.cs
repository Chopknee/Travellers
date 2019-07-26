using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopTrade : MonoBehaviour {

    public static UIShopTrade Instance {
        get {
            return inst;
        }
    } private static UIShopTrade inst;
    
    public PlayerData tradingPlayer;
    public Inventory shopInventory;

    public UIItemsList playerInventory;
    public UIItemsList uishopInventory;
    
    public UIItemsList sellWindow;
    public UIItemsList buyWindow;

    public Button btnExecuteTrade;
    public Button btnExitTrade;
    public Text txtSellValue;
    public Text txtBuyValue;
    public Text txtDirections;
    public Text txtPlayerGold;
    public Text txtNet;
    public Text shopName;
    public Text playerName;

    public void Awake() {
        inst = this;
        gameObject.SetActive(false);
    }

    public void ShowTradeWindow(string shopName, Inventory shopInventory, PlayerData playerToTradeWith) {
        if (gameObject.activeSelf) { return; }
        gameObject.SetActive(true);
        btnExecuteTrade.onClick.AddListener(ExecuteTrade);
        sellWindow.OnItemsChanged += OnItemsListChanged;
        buyWindow.OnItemsChanged += OnItemsListChanged;
        btnExitTrade.onClick.AddListener(ExitTrade);

        btnExecuteTrade.interactable = false;

        this.shopInventory = shopInventory;
        tradingPlayer = playerToTradeWith;

        sellWindow.associatedInventory = tradingPlayer.inventory;
        playerInventory.associatedInventory = tradingPlayer.inventory;

        //This is the shop side
        
        uishopInventory.associatedInventory = shopInventory;
        buyWindow.associatedInventory = shopInventory;
        txtPlayerGold.text = "+ On Hand: " + string.Format("{0, 0:D3}g", tradingPlayer.gold);
        this.shopName.text = shopName;
        playerName.text = playerToTradeWith.Name;

        //Fill the player inventory with all items
        foreach (ItemCard c in tradingPlayer.inventory.GetItems()) {
            c.owningInventory = tradingPlayer.inventory;
            playerInventory.InstantiateItem(c);
        }

        //Fill the shop inventory with all items
        foreach (ItemCard c in shopInventory.GetItems()) {
            c.owningInventory = shopInventory;
            uishopInventory.InstantiateItem(c);
        }
    }

    public void OnItemsListChanged(UIItemsList obj ) {
        //huh
        //Debug.Log("Items list has changed!!!");
        txtSellValue.text = "+ Sell: " + string.Format("{0, 0:D3}g", sellWindow.GoldValue);
        txtBuyValue.text = "- Buy: " + string.Format("{0, 0:D3}g", buyWindow.GoldValue);
        txtNet.text = "Net: " + string.Format("{0, 0:D3}g", sellWindow.GoldValue - buyWindow.GoldValue + tradingPlayer.gold);
        /*
            Rules for the trade to execute successfully
            There has to be items that are being purchased or sold.
            The total value must not be greater than the amount of gold the current player has.

            total value is calculated by taking the value of the sell window and subtracting it from the value of the sell window.

        */
        btnExecuteTrade.interactable = false;
        txtDirections.text = "";
        if (sellWindow.containedCards.Count != 0 || buyWindow.containedCards.Count != 0) {
            int value = sellWindow.GoldValue - buyWindow.GoldValue;
            Debug.Log("Value " + value);
            if (value < 0 && tradingPlayer.gold >= Mathf.Abs(value) || value >= 0) {
                //Player has enough gold to buy the items.
                btnExecuteTrade.interactable = true;
            } else {
                txtDirections.text = "Not enough gold!";
            }   
        }
    }



    public void ExecuteTrade() {
        //Complete the trade!!!
        txtPlayerGold.text = "+ On Hand: " + string.Format("{0, 0:D3}g", tradingPlayer.gold);
        int value = sellWindow.GoldValue - buyWindow.GoldValue;
        tradingPlayer.gold += value;
        tradingPlayer.inventory.PutItems(buyWindow.containedCards.ToArray(), shopInventory);
        shopInventory.PutItems(sellWindow.containedCards.ToArray(), tradingPlayer.inventory);
        //quick and dirty reset code ;)
        ExitTrade();
        ShowTradeWindow(shopName.text, shopInventory, tradingPlayer);
        //Quick and dirty fix ;;;)))
        txtSellValue.text = "+ Sell: " + string.Format("{0, 0:D3}g", sellWindow.GoldValue);
        txtBuyValue.text = "- Buy: " + string.Format("{0, 0:D3}g", buyWindow.GoldValue);
        txtNet.text = "Net: 000g";

    }

    public void ExitTrade() {
        //Closes the trade window.
        btnExecuteTrade.onClick.RemoveListener(ExecuteTrade);
        sellWindow.OnItemsChanged -= OnItemsListChanged;
        buyWindow.OnItemsChanged -= OnItemsListChanged;
        btnExitTrade.onClick.RemoveListener(ExitTrade);
        gameObject.SetActive(false);
        sellWindow.Cleanup();
        buyWindow.Cleanup();
        uishopInventory.Cleanup();
        playerInventory.Cleanup();
    }



}
