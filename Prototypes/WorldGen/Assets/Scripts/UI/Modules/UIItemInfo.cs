using BaD.Modules.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIItemInfo: MonoBehaviour {

    public ItemInstance item {
        get {
            return _item;
        }
        set {
            _item = value;
            UpdateInfo();
        }
    }

    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemValue;
    public TextMeshProUGUI itemDescription;

    private ItemInstance _item;

    void UpdateInfo() {
        ItemType details = _item.details;

        if (itemName != null) {    
            itemName.text = details.itemName;
        }

        if (itemValue != null) {
            itemValue.text = details.value + "g";
        }

        if (itemDescription != null) {
            itemDescription.text = details.description;
        }
    }
}
