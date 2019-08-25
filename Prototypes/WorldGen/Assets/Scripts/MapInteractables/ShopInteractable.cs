using BaD.Chopknee.Utilities;
using BaD.Modules;
using BaD.Modules.Networking;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaD.Modules.Terrain {
    public class ShopInteractable: MonoBehaviour {

        public GameObject HoverInfoGUIPrefab;
        private GameObject HoverInfoGUIInstance;

        public ShopData shopData;
        [HideInInspector]
        public string DisplayName;

        [HideInInspector]
        public NetInventory shopInventory;
        public int minItems = 0;
        public int maxItems = 10;
        
        private UIShopTrade shopGuI;

        bool isHighlighted;

        public float activationRadius;
        private float activationRadiusSquared;

        bool isCurrentNavTarget = false;

        int inventoryID;

        public void Start () {

            activationRadiusSquared = activationRadius * activationRadius;

            //Id used to reference this specific inventory.
            inventoryID = MainControl.Instance.GetStackSeed() + Choptilities.Vector3ToID(transform.position);
            //Send a request for the inventory. If it is already locally cached, this will run the callback before executing the next line, elsewise;
            //  the callback will not be run until the master client responds.
            NetworkedInventoryManager.Instance.RequestInventory(inventoryID, RequestInventoryCallback);//Id needs to be based on the current instance id stack
            //The reference to the shop gui
            shopGuI = MainControl.Instance.ShopUI.GetComponent<UIShopTrade>();
            //Making a copy of the shop data because idk
            shopData = shopData.GetNew();

            if (HoverInfoGUIPrefab != null) {
                HoverInfoGUIInstance = Instantiate(HoverInfoGUIPrefab);
                HoverInfoGUIInstance.GetComponent<UITargetObject>().target = transform;
                HoverInfoGUIInstance.SetActive(false);
                HoverInfoGUIInstance.transform.SetParent(MainControl.Instance.ActionConfirmationUI.transform);
            }
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
            //An inventory object has been received, now I need to determine if the start items should be dropped in??
            //if (shopInventory)
        }

        private void AddStartItems(int noiseSeed) {//Need to figure out how to know when the shop's items need to be initialized??

            //Select a random number of items to spawn based on the range of items allowed to spawn
            int itemsToSpawn = Noise.GetRandomRange(noiseSeed, minItems, maxItems);

            Item[] startitems = new Item[itemsToSpawn];
            //Fill the inventory with a random selection of items.
            for (int i = 0; i < itemsToSpawn; i++) {
                int randomItem = Noise.GetRandomRange(noiseSeed, 0, shopData.spawnableItems.Length);
                startitems[i] = NetworkedInventoryManager.Instance.MakeItemStruct(shopData.spawnableItems[randomItem]);
            }
            shopInventory.AddItems(startitems);
        }

        public string GetDisplayName () {
            return DisplayName;
        }

        public void Update () {
            if (isCurrentNavTarget && Input.GetButtonDown("Interact")) {
                if (!isHighlighted) {
                    isCurrentNavTarget = false;
                }
            }

            if (isCurrentNavTarget) {
                GameObject playerInst = DungeonManager.CurrentInstance.playerInstance;
                if (( playerInst.transform.position - transform.position ).sqrMagnitude < activationRadiusSquared) {
                    isCurrentNavTarget = false;
                    ActivateInstance();
                }
            }
        }

        public void OnMouseDown () {
            //Do the appropriate stuff here.
            GameObject playerInst = DungeonManager.CurrentInstance.playerInstance;
            if (( playerInst.transform.position - transform.position ).sqrMagnitude < activationRadiusSquared) {
                //Activate this thing.
                ActivateInstance();
            } else {
                isCurrentNavTarget = true;
                playerInst.GetComponent<PlayerMovement>().SetDestination(transform.position);

                //Move to this thing, then activate it.
            }
        }

        private void ActivateInstance() {
            shopGuI.ShowTradeWindow(DisplayName, shopInventory, MainControl.Instance.LocalPlayerData);
            shopGuI.OnClosed += ShopClosed;
            guiOpen = true;
        }

        public void OnMouseEnter () {
            if (HoverInfoGUIInstance != null) {
                HoverInfoGUIInstance.SetActive(true);
                HoverInfoGUIInstance.transform.Find("txtShopName").GetComponent<Text>().text = DisplayName;
                isHighlighted = true;
            }
        }

        public void OnMouseExit () {
            if (HoverInfoGUIInstance != null) {
                HoverInfoGUIInstance.SetActive(false);
                isHighlighted = false;
            }
        }

        public void OnDisable () {
            if (HoverInfoGUIInstance != null) {
                HoverInfoGUIInstance.SetActive(false);
                isHighlighted = false;
            }
        }

        public void OnValidate () {
            minItems = Mathf.Min(Mathf.Max(minItems, 0), maxItems - 1);//Cannot be less than 0, or greater than max items
            maxItems = Mathf.Max(minItems + 1, maxItems);//Cannot be less than min items
        }

        //Not sure how this is useful yet.
        bool guiOpen = false;
        public void ShopClosed () {
            guiOpen = false;
            shopGuI.OnClosed -= ShopClosed;
        }

        public void OnDrawGizmos () {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, activationRadius);
        }
    }
}