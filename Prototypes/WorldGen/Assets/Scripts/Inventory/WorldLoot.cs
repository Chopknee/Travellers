using BaD.Modules;
using BaD.Modules.Networking;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldLoot: MonoBehaviour, IPunObservable {

    public ItemType itemData;
    public ItemInstance netItemData {get;private set;}
    public GameObject itemDataGui;
    private GameObject itemDataGuiInst;
    public AudioClip pickupSound;

    public void Start () {
        if (NetInstanceManager.CurrentManager.isInstanceMaster) {
            //this is requesting a new item id
            netItemData = NetworkedInventoryManager.Instance.MakeItemStruct(itemData);
        }

        if (itemDataGui != null) {
            itemDataGuiInst = Instantiate(itemDataGui);
            itemDataGuiInst.GetComponent<UITargetObject>().target = transform;
            itemDataGuiInst.SetActive(false);
            //itemDataGuiInst.transform.SetParent(MainControl.Instance.ActionConfirmationUI.transform);
            itemDataGuiInst.GetComponentInChildren<Text>().text = itemData.itemName;
        }
    }

    public void OnMouseDown () {
        //Click on loot?
        if (pickupSound != null) {
            //Get the loot.
            MainControl.Instance.LocalPlayerData.Inventory.AddItem(netItemData);
            //Then destroy self.
            NetInstanceManager.CurrentManager.DestroyObject(gameObject);
        }
    }

    public void OnMouseOver () {
        if (itemDataGuiInst != null) {
            itemDataGuiInst.SetActive(true);
        }
    }

    public void OnMouseExit () {
        if (itemDataGuiInst != null) {
            itemDataGuiInst.SetActive(false);
        }
    }

    public void OnDestroy () {
        Destroy(itemDataGuiInst);
        //Set up the object that plays the clip we can hear.
        GameObject go = new GameObject("LootSound");
        AudioSource ass = go.AddComponent<AudioSource>();
        ass.playOnAwake = false;
        ass.clip = pickupSound;
        ass.Play();
        //Script that destroys the object after the clip plays
        DestroyInSeconds dis = go.AddComponent<DestroyInSeconds>();
        dis.time = pickupSound.length;
        dis.StartTimeout();
    }

    public void OnPhotonSerializeView ( PhotonStream stream, PhotonMessageInfo info ) {
        if (NetInstanceManager.CurrentManager.isInstanceMaster) {
            if (netItemData != null) {
                stream.SendNext(netItemData);
            }
        } else {
            netItemData = (ItemInstance) stream.ReceiveNext();
        }
    }

}
