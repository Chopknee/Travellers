using BaD.Modules.Networking;
using UnityEngine;

public class UIInventorySlot : MonoBehaviour  {

    public delegate void ItemDroppedHere ();
    public ItemDroppedHere OnItemDropped;

    public delegate void ItemPickedUp ();
    public ItemPickedUp OnItemPickedUp;

    private NetInventory inv;

    public void Start () {
        inv = GetComponentInParent<NetInventory>();
    }
}
