using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IMapInteractable {
    
    public ShopData shopData;
    public Inventory shopInventory;
    public int minItems = 0;
    public int maxItems = 10;

    public GameObject pointer;//If not assigned, 
    //public Vector3 pointerLocation = new Vector3();
    public Transform pointerLocation;

    private UIShopTrade shopGuI;

    public void Start() {
        shopGuI = MainControl.Instance.ShopGUI.GetComponent<UIShopTrade>();
        shopData = shopData.GetNew();//Workaround type of thing...
        //Initialize a new instance of inventory.
        shopInventory = new Inventory();

        if (shopData.ShopName == "") {
            shopData.ShopName = Noise.GetRandomString(OverworldControl.Instance.NoiseSeed, Noise.serverNames) + " " + Noise.GetRandomString(OverworldControl.Instance.NoiseSeed, Noise.shopTitles);
        }
        //Select a random number of items to spawn based on the range of items allowed to spawn
        int itemsToSpawn = Noise.GetRandomRange(OverworldControl.Instance.NoiseSeed, minItems, maxItems);

        //Fill the inventory with a random selection of items.
        for (int i = 0; i < itemsToSpawn; i++) {
            int randomItem = Noise.GetRandomRange(OverworldControl.Instance.NoiseSeed, 0, shopData.spawnableItems.Length);
            shopInventory.PutItem(shopData.spawnableItems[randomItem], null);
        }

        if (OverworldControl.Instance.BuildingPointer != null && pointer == null) {
            pointer = OverworldControl.Instance.BuildingPointer;
        }

    }

    public string GetDisplayName() {
        return shopData.ShopName;
    }

    public void Interact(Player player) {
        //Do the appropriate stuff here.
        shopGuI.ShowTradeWindow(shopData.ShopName, shopInventory, player.data);
        shopGuI.OnClosed += ShopClosed;
        guiOpen = true;
    }

    public void OnValidate() {
        minItems = Mathf.Min(Mathf.Max(minItems, 0), maxItems-1);//Cannot be less than 0, or greater than max items
        maxItems = Mathf.Max(minItems+1, maxItems);//Cannot be less than min items
    }

    public InteractResult TryInteract(Player player) {
        return new InteractResult(true);
    }

    public void SetHighlight(bool state) {
        Debug.Log("Highight activated!!!");
        //Enable the pointer
        if (state) {
            pointer.transform.position = pointerLocation.transform.position;
            pointer.SetActive(true);
        } else {
            //Prevents this call from overriding previous calls to hide the pointer
            if (pointer.transform.position ==  pointerLocation.transform.position) {
                pointer.SetActive(false);
            }
        }
    }

    bool guiOpen = false;

    public void ShopClosed() {
        guiOpen = false;
        shopGuI.OnClosed -= ShopClosed;
    }

    public bool InteractionComplete(Player player) {
        return !guiOpen;
    }

    public string GetActionName() {
        return "Trade With " + shopData.ShopName;
    }

    public string GetShortActionName() {
        return "Trade";
    }
}