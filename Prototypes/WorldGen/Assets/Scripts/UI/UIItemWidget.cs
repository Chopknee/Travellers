using BaD.UI.DumpA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemWidget : IUIItemcard {


    [Header("UI Components")]
    public Text nameText;
    public Text valueText;
    public Image itemImage;

    public override void CardDataChanged(ItemCard cd) {
        if (nameText != null) {
            nameText.text = cd.itemName;
        }
        if (valueText != null) {
            valueText.text = string.Format("{0, 0:D1}g", cd.value);
        }

        itemImage.sprite = cd.itemSprite;
    }
}
