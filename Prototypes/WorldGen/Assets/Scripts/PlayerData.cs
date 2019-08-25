using BaD.Modules.Networking;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {

    public GameObject playerGameobjectRef { get; private set; }

    public GameObject playerInventoryGameobjectRef { get; private set; }

    public NetInventory Inventory {
        get {
            if (playerInventoryGameobjectRef != null) {
                return playerInventoryGameobjectRef.GetComponent<NetInventory>();
            } else {
                return null;
            }
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

    public PlayerData(GameObject playerRef) {
        playerGameobjectRef = playerRef;
        NetworkedInventoryManager.Instance.RequestInventory(PhotonNetwork.LocalPlayer.ActorNumber, OnInventoryRequestFulfilled);
    }

    public void OnInventoryRequestFulfilled(GameObject go, bool needsInitialize) {
        //In this case needs initialize can be ignored.
        playerInventoryGameobjectRef = go;
        Inventory.OnItemsUpdated += OnInventoryChanged;
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
