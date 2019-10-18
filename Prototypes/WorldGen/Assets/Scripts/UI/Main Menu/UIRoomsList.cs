using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomsList : MonoBehaviourPunCallbacks {

    public Button refreshButton;

    private void Start () {
        if (refreshButton != null) {
            refreshButton.onClick.AddListener(RefreshRoomList);
        }
    }

    new void OnEnable () {
        RefreshRoomList();
    }

    void RefreshRoomList () {
        Debug.Log("<color=cyan>Attempting to join lobby...</color>");
        //if (PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "")) {
        //    Debug.Log("Get custom room list executed.");
        //}
        if (PhotonNetwork.JoinLobby()) {
            Debug.Log("<color=cyan>Successfully joined the lobby, refreshing rooms...</color>");
            return;
        }
        Debug.Log("<color=cyan>Failed to refresh room list. Unknown reason.</color>");
    }

    public override void OnRoomListUpdate ( List<RoomInfo> roomList ) {
        Debug.Log("<color=cyan>Room list refreshed.</color>");
        foreach (RoomInfo room in roomList) {
            Debug.Log(room.Name);
        }
    }
}
