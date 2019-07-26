using UnityEngine;

public class Trader : Inventory, IMapInteractable {

    public ItemCard[] startingItemPool;//The items this trader spawns with
    public int MinStartItems = 5;
    public int MaxStartItems = 15;
    public string displayName;
    public void Start() {
        //Randomly fill the inventory of the shop with items.
        int itemsToSpawnWith = Noise.GetRandomRange(Control.Instance.NoiseSeed, MinStartItems, MaxStartItems);

        for (int i = 0; i < itemsToSpawnWith; i++) {
            PutItem(startingItemPool[Noise.GetRandomRange(Control.Instance.NoiseSeed, 0, startingItemPool.Length)], null);
        }
        
    }

    //public 
    public void Interact(Player p) {
        //The trade interaction menu should pop up.
        Debug.Log("I am a market trader!");
    }

    public string GetDisplayName() {
        return displayName;
    }

}