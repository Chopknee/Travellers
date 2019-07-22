using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInventory : Inventory {

    public GameObject ItemCardPrefab;
    public ItemCard[] cardsToSpawn;

    // Update is called once per frame
    void Update() {
        if (ItemCardPrefab != null) {
            if (GetComponentInChildren<UICard>() == null) {
                //Spawn a new random card.
                GameObject go = Instantiate(ItemCardPrefab, GetComponentInChildren<UIInventorySlot>().transform);
                ItemCard c = cardsToSpawn[Random.Range(0, cardsToSpawn.Length - 1)];
                cards.Add(c);
                go.GetComponent<ItemCardObject>().CardData = cardsToSpawn[Random.Range(0, cardsToSpawn.Length - 1)];

            }
        }
    }
}
