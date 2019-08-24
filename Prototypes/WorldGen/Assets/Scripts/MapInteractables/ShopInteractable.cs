using BaD.Modules;
using BaD.Modules.Networking;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaD.Modules.Terrain {
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(NetInventory))]
    public class ShopInteractable: MonoBehaviour, IPunObservable {

        public GameObject HoverInfoGUIPrefab;
        private GameObject HoverInfoGUIInstance;

        public ShopData shopData;
        public string DisplayName;
        public float structureRadius;

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

        public bool isHighlighted;

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
            structureRadius = GetComponent<StructureDataLink>().structureData.radius;
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

        public void OnMouseDown () {
            //Do the appropriate stuff here.
            shopGuI.ShowTradeWindow(DisplayName, shopInventory, MainControl.Instance.LocalPlayerData);
            shopGuI.OnClosed += ShopClosed;
            guiOpen = true;
        }

        public void OnMouseEnter () {
            if (HoverInfoGUIInstance != null) {
                HoverInfoGUIInstance.SetActive(true);
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

        bool guiOpen = false;

        public void ShopClosed () {
            guiOpen = false;
            shopGuI.OnClosed -= ShopClosed;
        }

        public void OnPhotonSerializeView ( PhotonStream stream, PhotonMessageInfo info ) {
            if (stream.IsWriting) {//Only send the data if the instance is miine????
                stream.SendNext(DisplayName);
                stream.SendNext(structureRadius);
            } else {
                //if ()
                DisplayName = (string) stream.ReceiveNext();
                structureRadius = (float) stream.ReceiveNext();

            }
        }
    }
}