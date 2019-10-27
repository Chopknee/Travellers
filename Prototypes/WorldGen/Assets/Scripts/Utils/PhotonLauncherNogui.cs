using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonLauncherNogui : MonoBehaviourPunCallbacks {

    //Auto connect and get photon things working so I don't have to go through the menu launcher!!!!
    string gameVersion = "testing";

    public bool offlineMode = true;
    public string roomName = "TestRoom";


    void Start() {
        Debug.LogFormat("<color=cyan>Setting game version {0}.</color>", gameVersion);
        //Set the game version
        PhotonNetwork.GameVersion = gameVersion;
        //Connect to the photon lobby
        
        if (offlineMode) {
            Debug.Log("<color=cyan>Setting offline mode...</color>");
            PhotonNetwork.OfflineMode = true;
        } else {
            Debug.Log("<color=cyan>Connecting to photon servers...</color>");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster() {
        //This is automatically run if offline mode is set.
        Debug.Log("Connected to master server.");
        if (PhotonNetwork.OfflineMode) {
            //Do I just need to make a new room, or what?
            if (PhotonNetwork.CreateRoom(roomName)) {
                Debug.Log("Creating a new room!");
            }
        } else {
            if (PhotonNetwork.JoinRoom(roomName)) {
                Debug.Log("Joining room...");
            } else {
                if (PhotonNetwork.CreateRoom(roomName)) {
                    Debug.Log("Could not join room, creating one in stead.");
                }
            }
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        Debug.Log("Failed to join a room, creating one in stead.");
        PhotonNetwork.CreateRoom(roomName);
    }

    public override void OnCreatedRoom() {
        Debug.Log("<color=cyan>Room created successfully.</color>");
    }

    public override void OnJoinedRoom() {
        Debug.LogFormat("Joined room successfully.");
        //Do the stuff for the game??
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
