using BaD.Modules;
using BaD.Modules.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShopActivation : MonoBehaviour {

    public GameObject shopUIInstance;
    public ShopData shopData;
    UIShopTrade shopGuI;

    [HideInInspector]
    public NetInventory shopInventory;
    public int minItems = 0;
    public int maxItems = 10;

    int inventoryID;

    PlayerData localPlayer;

    public void Start() {
        Debug.Log("Starting test shop.");
        localPlayer = new PlayerData(null);
        localPlayer.gold = 100;

        MainControl.LocalPlayerData = localPlayer;

        //Id used to reference this specific inventory.
        inventoryID = 69;//MainControl.Instance.GetStackSeed() + Choptilities.Vector3ToID(transform.position);
        //Send a request for the inventory. If it is already locally cached, this will run the callback before executing the next line, elsewise;
        //  the callback will not be run until the master client responds.
        NetworkedInventoryManager.Instance.RequestInventory(inventoryID, RequestInventoryCallback);//Id needs to be based on the current instance id stack

        //Making a copy of the shop data because idk
        shopData = shopData.GetNew();
    }

    public void RequestInventoryCallback(GameObject inv, bool needsInitialize) {
        Debug.Log("Inventory request callback has been 'called back'.");
        //Once the callback is run, we assign the inventory instance object
        shopInventory = inv.GetComponent<NetInventory>();

        int noiseSeed = inventoryID;// + OverworldControl.Instance.NoiseSeed;//To ensure it never overlaps the existing map seed.
        Noise.Reset(noiseSeed);//Even if we get the same seed, it won't cause issues with differnet access times for clients.

        //If the inventory has not been initialized, go ahead and initialize it.
        if (needsInitialize) {
            AddStartItems(noiseSeed);
        }

        //Automagically instantiate the thing.
        OpenShop();
    }

    private void AddStartItems(int noiseSeed) {//Need to figure out how to know when the shop's items need to be initialized??

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
        if (shopUIInstance == null) {
            Debug.Log("Could not open shop ui, instance has not been set!", this);
            return;
        }
        shopGuI = shopUIInstance.GetComponent<UIShopTrade>();
        shopGuI.ShowTradeWindow("Test Schopp", shopInventory, localPlayer);
        shopGuI.OnClosed += ShopClosed;
    }


    public void ShopClosed() {
        Debug.Log("Shop instance was closed!");
        shopGuI.OnClosed -= ShopClosed;
        OpenShop();
    }


}
