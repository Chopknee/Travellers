using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventorySlot : MonoBehaviour  {

    public delegate void ItemDroppedHere ();
    public ItemDroppedHere OnItemDropped;

    public delegate void ItemPickedUp ();
    public ItemPickedUp OnItemPickedUp;

    private Inventory inv;

    public void Start () {
        inv = GetComponentInParent<Inventory>();
    }
}
