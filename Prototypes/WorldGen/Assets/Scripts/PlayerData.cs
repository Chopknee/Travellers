using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {

    public static int MaxBaseInventory = 10;

    public GameObject playerGameobjectRef { get; private set; }
    public NetInventory Inventory {
        get {
            return playerGameobjectRef.GetComponent<NetInventory>();
        }
    }
    public delegate void PlayerGoldChanged(PlayerData player);
    public PlayerGoldChanged OnPlayerGoldChanged;

    public delegate void PlayerNameChanged(PlayerData player);
    public PlayerNameChanged OnPlayernameChanged;

    public PlayerData(GameObject playerRef) {
        playerGameobjectRef = playerRef;
    }

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
