using BaD.Modules;
using BaD.UI.DumpA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipableContextMenu : MonoBehaviour {

    public Button northButton;
    public Button southButton;
    public Button eastButton;

    public UIPlayerInventory inventoryWindow;

    void Start() {
        northButton.onClick.AddListener(OnNorth);
        southButton.onClick.AddListener(OnSouth);
        eastButton.onClick.AddListener(OnEast);
    }

    void OnNorth() {
        Debug.Log("EQUIPPING NORTH");
        MainControl.LocalPlayerData.northEquppedItem = inventoryWindow.currentSelectedItem;
    }

    void OnSouth() {
        MainControl.LocalPlayerData.southEquippedItem = inventoryWindow.currentSelectedItem;
    }

    void OnEast() {
        MainControl.LocalPlayerData.eastEquippedItem = inventoryWindow.currentSelectedItem;
    }
}
