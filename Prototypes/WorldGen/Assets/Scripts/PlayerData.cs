using BaD.Modules.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {

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

    public delegate void PlayerActionsTakenChanged ( PlayerData player );
    public PlayerActionsTakenChanged OnActionsTakenChanged;

    private string name;
    private int takenActions = 0;
    private int heldGold = 0;
    private Collection[] actionIncreaseFilter;

    public PlayerData(GameObject playerRef, Collection[] actionIncreaseFilter) {
        playerGameobjectRef = playerRef;
        Inventory.OnItemsUpdated += OnInventoryChanged;
        this.actionIncreaseFilter = actionIncreaseFilter;
    }

    //Generic class holding information about a player.
    public int gold {
        get {
            return heldGold;
        }
        set {
            heldGold = value;
            OnPlayerGoldChanged?.Invoke(this);
        }
    }
    
    public string Name {
        get {
            return name;
        }
        set {
            name = value;
            OnPlayernameChanged?.Invoke(this);
            
        }
    }

    private int maxActions = 0;

    public int MaxActions {
        get {
            Item[] filtered = BaD.Modules.Inventory.Inventory.GetItemsByGroup(Inventory.Items, actionIncreaseFilter);

            if (2 + filtered.Length > maxActions) {
                maxActions = 2 + filtered.Length;
            }
            return maxActions;
        }
    }

    public int ActionsTaken {
        get {
            return takenActions;
        }
        set {
            takenActions = value;
            OnActionsTakenChanged?.Invoke(this);
        }
    }
    
    private void OnInventoryChanged( int originalRequestID, Item[] addedItems, Item[] removedItems ) {
        OnActionsTakenChanged?.Invoke(this);
    }

    public void ResetTurn() {
        takenActions = 0;
        maxActions = 0;
        OnActionsTakenChanged?.Invoke(this);
    }

}
