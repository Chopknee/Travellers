using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteShop : MonoBehaviour, IUIDropZone {

    public GameObject ItemCardPrefab;
    public ItemCard[] cardsToSpawn;

    public void Start() {
        GameObject go = Instantiate(ItemCardPrefab);
        ItemCard c = cardsToSpawn[Random.Range(0, cardsToSpawn.Length - 1)];
        go.GetComponent<ItemWidget>().CardData = c;
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
    }

    public bool TryDropItem(UIDraggable item) {
        //Debug.Log("Item received!");
        return false;//This is not a droppable inventory
    }

    public bool TryGrabItem(UIDraggable item) {
        Debug.Log("Item taken from infinite inventory!");
        return true;
    }

    public void TransferComplete(UIDraggable item) {
        Debug.Log("New item spawned!");
        GameObject go = Instantiate(ItemCardPrefab);
        ItemCard c = cardsToSpawn[Random.Range(0, cardsToSpawn.Length - 1)];
        go.GetComponent<ItemWidget>().CardData = c;
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
    }

}
