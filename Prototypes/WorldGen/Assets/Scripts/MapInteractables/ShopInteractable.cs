using BaD.Chopknee.Utilities;
using BaD.Modules.Networking;
using UnityEngine;

namespace BaD.Modules.Terrain {
    public class ShopInteractable: Interactable {

        public ShopData shopData;
        [HideInInspector]
        public string DisplayName;

        [HideInInspector]
        public NetInventory shopInventory;
        public int minItems = 0;
        public int maxItems = 10;

        public GameObject shopUIPrefab;
        private GameObject shopUIInstance;

        private UIShopTrade shopGuI;

        int inventoryID;

        public override void Start () {


            //Id used to reference this specific inventory.
            inventoryID = MainControl.Instance.GetStackSeed() + Choptilities.Vector3ToID(transform.position);
            //Send a request for the inventory. If it is already locally cached, this will run the callback before executing the next line, elsewise;
            //  the callback will not be run until the master client responds.
            NetworkedInventoryManager.Instance.RequestInventory(inventoryID, RequestInventoryCallback);//Id needs to be based on the current instance id stack
            
            //Making a copy of the shop data because idk
            shopData = shopData.GetNew();

            base.Start();
        }

        public void RequestInventoryCallback(GameObject inv, bool needsInitialize) {
            Debug.Log("Inventory request callback has been 'called back'.");
            //Once the callback is run, we assign the inventory instance object
            shopInventory = inv.GetComponent<NetInventory>();

            int noiseSeed = inventoryID + OverworldControl.Instance.NoiseSeed;//To ensure it never overlaps the existing map seed.
            Noise.Reset(noiseSeed);//Even if we get the same seed, it won't cause issues with differnet access times for clients.
            DisplayName = Noise.GetRandomString(noiseSeed, Noise.serverNames) + " " + Noise.GetRandomString(noiseSeed, Noise.shopTitles);

            //If the inventory has not been initialized, go ahead and initialize it.
            if (needsInitialize) {
                AddStartItems(noiseSeed);
            }
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

        public override void DoInteraction () {
            if (shopUIInstance == null) {
                shopUIInstance = Instantiate(shopUIPrefab);
            }
            shopGuI = shopUIInstance.GetComponent<UIShopTrade>();
            shopGuI.ShowTradeWindow(DisplayName, shopInventory, MainControl.LocalPlayerData);
            shopGuI.OnClosed += ShopClosed;
        }

        public override string GetShopName() {
            return DisplayName;
        }

        //Not sure how this is useful yet.
        public void ShopClosed () {
            shopGuI.OnClosed -= ShopClosed;
            //Destroy(shopUIInstance);
        }
    }
}