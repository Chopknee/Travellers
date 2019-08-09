using BaD.Modules;
using BaD.Modules.Networking;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaD.Modules.Terrain {
    public class ShopInteractable: MonoBehaviour, IMapInteractable, IPunObservable {

        public ShopData shopData;
        public string DisplayName;

        [HideInInspector]
        public NetInventory shopInventory;
        public int minItems = 0;
        public int maxItems = 10;

        [SerializeField]
#pragma warning disable 0649
        public GameObject pointer;//If not assigned, 
        [SerializeField]
#pragma warning disable 0649
        private Transform pointerLocation;
        private UIShopTrade shopGuI;

        public void Start () {
            shopInventory = GetComponent<NetInventory>();
            shopGuI = MainControl.Instance.ShopUI.GetComponent<UIShopTrade>();
            shopData = shopData.GetNew();//Workaround type of thing...
            if (OverworldControl.Instance.BuildingPointer != null && pointer == null) {
                pointer = OverworldControl.Instance.BuildingPointer;
            }

            //Only the master client may generate the initial list of starting items.
            if (PhotonNetwork.IsMasterClient) {
                Invoke("AddStartItems", 0.1f);
            }
        }

        private void AddStartItems() {
            int noiseSeed = OverworldControl.Instance.NoiseSeed - 100;//-100 can be for the shops to prevent issues with delayed spawns

            DisplayName = Noise.GetRandomString(noiseSeed, Noise.serverNames) + " " + Noise.GetRandomString(noiseSeed, Noise.shopTitles);
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

        public void Interact ( Player player ) {
            //Do the appropriate stuff here.
            shopGuI.ShowTradeWindow(DisplayName, shopInventory, player.Data);
            shopGuI.OnClosed += ShopClosed;
            guiOpen = true;
        }

        public void OnValidate () {
            minItems = Mathf.Min(Mathf.Max(minItems, 0), maxItems - 1);//Cannot be less than 0, or greater than max items
            maxItems = Mathf.Max(minItems + 1, maxItems);//Cannot be less than min items
        }

        public InteractResult TryInteract ( Player player ) {
            return new InteractResult(true);
        }

        public void SetHighlight ( bool state ) {
            //Enable the pointer
            if (state) {
                pointer.transform.position = pointerLocation.transform.position;
                pointer.SetActive(true);
            } else {
                //Prevents this call from overriding previous calls to hide the pointer
                if (pointer.transform.position == pointerLocation.transform.position) {
                    pointer.SetActive(false);
                }
            }
        }

        bool guiOpen = false;

        public void ShopClosed () {
            guiOpen = false;
            shopGuI.OnClosed -= ShopClosed;
        }

        public bool InteractionComplete ( Player player ) {
            return !guiOpen;
        }

        public string GetActionName () {
            return "Trade With " + DisplayName;
        }

        public string GetShortActionName () {
            return "Trade";
        }

        public void OnPhotonSerializeView ( PhotonStream stream, PhotonMessageInfo info ) {
            if (stream.IsWriting) {//Only send the data if the instance is miine????
                stream.SendNext(DisplayName);
            } else {
                //if ()
                DisplayName = (string) stream.ReceiveNext();
                Debug.Log(DisplayName);
            }
        }
    }
}