using BaD.Modules;
using BaD.Modules.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestShopActivation : MonoBehaviour {

    public GameObject shopUIInstance;
    public ShopData shopData;
    UIShopTrade shopGuI;

    [HideInInspector]
    public NetInventory shopInventory;
    public int minItems = 0;
    public int maxItems = 10;

    public Slider shopID;

    int inventoryID;

    PlayerData localPlayer;

    public void Start() {
        localPlayer = new PlayerData(null);
        localPlayer.gold = 100;
        MainControl.LocalPlayerData = localPlayer;
    }

    public void RequestInventoryCallback(GameObject inv, bool needsInitialize) {
        Debug.Log("CALLED THE FUCK BACK FROM REQUESTING INVENTORY");
        //Once the callback is run, we assign the inventory instance object
        shopInventory = inv.GetComponent<NetInventory>();

        int noiseSeed = inventoryID;// + OverworldControl.Instance.NoiseSeed;//To ensure it never overlaps the existing map seed.
        Noise.Reset(noiseSeed);//Even if we get the same seed, it won't cause issues with differnet access times for clients.

        //If the inventory has not been initialized, go ahead and initialize it.
        if (needsInitialize) {
            AddStartItems(noiseSeed);
        }

        if (shopUIInstance == null) {
            Debug.Log("Could not open shop ui, instance has not been set!", this);
            return;
        }
        shopGuI = shopUIInstance.GetComponent<UIShopTrade>();
        shopGuI.OnClosed += ShopClosed;
        shopGuI.ShowTradeWindow("Test Shop", shopInventory, localPlayer);
        shopUIInstance.SetActive(true);
    }

    private void AddStartItems(int noiseSeed) {//Need to figure out how to know when the shop's items need to be initialized??
        Debug.Log("ADDING ITEMS");
        //Select a random number of items to spawn based on the range of items allowed to spawn
        int itemsToSpawn = Noise.GetRandomRange(noiseSeed, minItems, maxItems);

        ItemInstance[] startitems = new ItemInstance[itemsToSpawn];
        //Fill the inventory with a random selection of items.
        for (int i = 0; i < itemsToSpawn; i++) {
            int randomItem = Noise.GetRandomRange(noiseSeed, 0, shopData.spawnableItems.Length);
            startitems[i] = NetworkedInventoryManager.Instance.MakeItemStruct(shopData.spawnableItems[randomItem]);
        }
        shopInventory.AddItems(startitems);
    }

    public void OpenShop() {
        Debug.Log("Starting test shop.");
        inventoryID = (int)shopID.value;
        NetworkedInventoryManager.Instance.RequestInventory(inventoryID, RequestInventoryCallback);
    }


    public void ShopClosed() {
        Debug.Log("Shop instance was closed!");
        shopGuI.OnClosed -= ShopClosed;
        shopUIInstance.SetActive(false);
    }


}
