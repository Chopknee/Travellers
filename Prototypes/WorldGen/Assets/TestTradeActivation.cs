using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTradeActivation : MonoBehaviour { 

    public int playerGold = 10;
    public ItemCard[] playerInventory;
    public ItemCard[] shopInventory;

    public GameObject shopGUI;
    // Start is called before the first frame update
    void Start() {
        shopGUI = GameObject.FindGameObjectWithTag("ShopGUI");
    }

    void OnMouseDown() {
        //PlayerData pd = new PlayerData();
        //pd.gold = playerGold;
        //pd.inventory.PutItems(playerInventory, null);

        NetInventory shopInv = new NetInventory();
        //shopInv.PutItems(shopInventory, null);
        //UIShopTrade.Instance.ShowTradeWindow("", shopInv, pd);

        //Debug.Log("I think this works!");
    }
}
