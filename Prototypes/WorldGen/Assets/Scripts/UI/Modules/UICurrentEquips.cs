using BaD.Modules;
using BaD.Modules.Networking;
using BaD.UI.DumpA;
using UnityEngine;

public class UICurrentEquips : MonoBehaviour {

    public UIItemChit northItem;
    public UIItemChit southItem;
    public UIItemChit eastItem;

    void Start() {
        MainControl.LocalPlayerData.OnEquipsChanged += OnEquipsChanged;
    }

    void OnEquipsChanged(Enums.Directions direction, ItemInstance item) {
        Debug.Log("Showing equipped item.");
        switch (direction) {
            case Enums.Directions.North:
                northItem.ItemData = item.details;
                break;
            case Enums.Directions.South:
                southItem.ItemData = item.details;
                break;
            case Enums.Directions.East:
                eastItem.ItemData = item.details;
                break;
        }
    }
}
