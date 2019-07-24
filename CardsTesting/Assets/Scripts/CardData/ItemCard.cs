using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu()]
public class ItemCard: UpdateableData {

    public enum Tier { Tier1, Tier2, Tier3 };
    [Header("Stats")]
    public string itemName;
    [TextArea]
    public string description;
    [Range(0, 20)]
    public int value;
    public Sprite itemSprite;
    public Tier cardTier;
    //public List<Set> sets;
    public Collection[] collections;

#if UNITY_EDITOR

    protected override void OnValidate () {

        base.OnValidate();

    }

#endif
}


