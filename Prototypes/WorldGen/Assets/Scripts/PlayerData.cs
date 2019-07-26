using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {

    public static int MaxBaseInventory = 10;


    public delegate void PlayerGoldChanged(PlayerData player);
    public PlayerGoldChanged OnPlayerGoldChanged;

    public delegate void PlayerNameChanged(PlayerData player);
    public PlayerNameChanged OnPlayernameChanged;

    //Generic class holding information about a player.
    public int gold {
        get {
            return heldGold;
        }
        set {
            //Do some extra things here I guess.
            OnPlayerGoldChanged?.Invoke(this);
            heldGold = value;
        }
    }
    private int heldGold = 0;

    public int maxInventory {
        get {
            return MaxBaseInventory;
        }
    }

    public Inventory inventory = new Inventory();

    public string Name {
        get {
            return name;
        }
        set {
            OnPlayernameChanged?.Invoke(this);
            name = value;
        }
    }

    private string name;


}
