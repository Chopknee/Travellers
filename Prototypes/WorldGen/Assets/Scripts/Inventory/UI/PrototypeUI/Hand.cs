using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    //Makes a ui visualization of the player's inventory
    Inventory playerInventory;
    public GameObject cardPrefab;
    private List<GameObject> itemCards;
    private GameObject[] cardSpots;
    // Start is called before the first frame update
    void Start() {
        playerInventory = GetComponent<Inventory>();
        //Load the hand with the cards???
        itemCards = new List<GameObject>();
        LoadHand();
    }

    // Update is called once per frame
    void Update() {
        //FuckYou();
    }

    public void LoadHand() {
        if (itemCards.Count < playerInventory.maxSize) {
            for (int i = itemCards.Count; i < playerInventory.maxSize; i++) {
                itemCards.Add(Instantiate(cardPrefab, transform));
                itemCards[i].SetActive(false);
            }
        }
        ItemCard[] cards = playerInventory.GetItems();
        for (int i = 0; i < playerInventory.maxSize; i++) {
            if (i < playerInventory.ItemCount - 1) {
                itemCards[i].SetActive(true);
                itemCards[i].GetComponent<ItemCardObject>().CardData = cards[i];
            } else {
                itemCards[i].SetActive(false);
            }
        }
    }
}
