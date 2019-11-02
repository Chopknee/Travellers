using BaD.Modules.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu()]
public class ItemType: UpdateableData {

    public enum Tier { Tier1, Tier2, Tier3 };
    public enum UseType { Junk, Equipable, Consumable };
    [Header("Stats")]
    public string itemName;
    [TextArea]
    public string description;
    [Range(0, 20)]
    public int value;
    public Sprite itemSprite;
    public Tier cardTier;
    public UseType usageType;
    public ItemModifier[] collections;
    public GameObject worldItemPrefab;//The gameobject that will represent this item in the world.


    public ItemType GetNew() {
        ItemType ic = (ItemType)ScriptableObject.CreateInstance("ItemCard");
        //ItemCard ic = new ItemCard();
        ic.itemName = itemName;
        ic.description = description;
        ic.value = value;
        ic.itemSprite = itemSprite;
        ic.cardTier = cardTier;
        ic.collections = new ItemModifier[collections.Length];
        for (int i = 0; i < collections.Length; i++) {
            ic.collections[i] = collections[i];
        }
        return ic;
    }

    public ItemInstance NewNetInstance() {
        return NetworkedInventoryManager.Instance.MakeItemStruct(this);
    }

#if UNITY_EDITOR

    protected override void OnValidate () {

        base.OnValidate();

    }

#endif
}
