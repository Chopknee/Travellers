using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IMapInteractable {
    
    public ShopData shopData;
    public Inventory shopInventory;
    public int minItems = 0;
    public int maxItems = 10;

    public void Start() {

        shopData = shopData.GetNew();//Workaround type of thing...
        //Initialize a new instance of inventory.
        shopInventory = new Inventory();

        if (shopData.ShopName == "") {
            shopData.ShopName = Noise.GetRandomString(Control.Instance.NoiseSeed, Noise.serverNames) + " " + Noise.GetRandomString(Control.Instance.NoiseSeed, Noise.shopTitles);
        }
        //Select a random number of items to spawn based on the range of items allowed to spawn
        int itemsToSpawn = Noise.GetRandomRange(Control.Instance.NoiseSeed, minItems, maxItems);

        //Fill the inventory with a random selection of items.
        for (int i = 0; i < itemsToSpawn; i++) {
            int randomItem = Noise.GetRandomRange(Control.Instance.NoiseSeed, 0, shopData.spawnableItems.Length);
            shopInventory.PutItem(shopData.spawnableItems[randomItem], null);
        }
    }

    public string GetDisplayName() {
        return shopData.ShopName;
    }

    public void Interact(Player player) {
        //Do the appropriate stuff here.
        UIShopTrade.Instance.ShowTradeWindow(shopData.ShopName, shopInventory, player.data);
    }

    public void OnValidate() {
        minItems = Mathf.Min(Mathf.Max(minItems, 0), maxItems-1);//Cannot be less than 0, or greater than max items
        maxItems = Mathf.Max(minItems+1, maxItems);//Cannot be less than min items
    }

}
