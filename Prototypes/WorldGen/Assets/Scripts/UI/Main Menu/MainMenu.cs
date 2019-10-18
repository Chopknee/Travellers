using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviourPunCallbacks {

    public string gameVersion = "0.1";

    public Button playOnlineButton;
    public Button playOfflineButton;
    public Button quitButton;
    public RectTransform playOnlinePanel;

    public void Awake () {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void Start () {
        Debug.LogFormat("<color=cyan>Setting game version {0}.</color>", gameVersion);
        //Set the game version
        PhotonNetwork.GameVersion = gameVersion;
        //Connect to the photon lobby
        Debug.Log("<color=cyan>Connecting to photon servers...</color>");
        PhotonNetwork.ConnectUsingSettings();

        playOnlineButton.onClick.AddListener(OnPlayOnlineClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
        playOnlineButton.enabled = false;
    }

    public void OnPlayOnlineClicked() {
        playOnlinePanel.gameObject.SetActive(true);
    }

    public void OnQuitClicked() {
        Application.Quit();
    }

    public void HostGame() {

        //Creates a new game based on existing player prefs
        if (PhotonNetwork.CreateRoom(PlayerPrefs.GetString("HostRoomName"))) {
            Debug.Log("<color=cyan>Attempting to create new room...</color>");
            return;
        }
        Debug.Log("<color=cyan>Failed to create new room. Unknown cause.</color>");
    }

    public void JoinGame() {
        string name = PlayerPrefs.GetString("JoinRoomName");
        if (name != "") {
            if (PhotonNetwork.JoinRoom(name)) {
                Debug.LogFormat("Attempting to join room {0}.", name);
            } else {
                Debug.LogFormat("Could not join room {0}.", name);
            }
        } else {
            Debug.LogFormat("Could not join room, no name was specified.");
        }
    }

    public override void OnConnected () {
        Debug.Log("<color=cyan>OnConnected callback was executed.</color>");
    }

    public override void OnConnectedToMaster () {
        Debug.Log("<color=cyan>Connection to photon master server was successful.</color>");
        playOnlineButton.enabled = true;
    }

    public override void OnDisconnected ( DisconnectCause cause ) {
        Debug.LogFormat("<color=cyan>Disconnected from servers. Cause: <color=red>{0}</color>.</color>", cause.ToString());
    }

    public override void OnCreatedRoom () {
        Debug.Log("<color=cyan>Room created successfully.</color>");
    }

    public override void OnCreateRoomFailed ( short returnCode, string message ) {
        Debug.LogFormat("<color=cyan>Failed to create room. Cause: {0}.", message);
    }

    public override void OnJoinedRoom() {
        Debug.LogFormat("Joined room successfully.");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
            PhotonNetwork.LoadLevel("Game");
        }
    }

    public override void OnJoinRoomFailed ( short returnCode, string message ) {
        Debug.LogFormat("Could not join room. Reason; {0}", message);
    }

    //public void OnRegionListReceived ( RegionHandler regionHandler ) {
    //    Debug.Log("<color=cyan>OnRegionListReceived callback was executed.</color>");
    //}

    //public void OnCustomAuthenticationResponse ( Dictionary<string, object> data ) {
    //    Debug.Log("<color=cyan>OnCustomAuthenticationResponse callback was executed.</color>");
    //}

    //public void OnCustomAuthenticationFailed ( string debugMessage ) {
    //    Debug.Log("<color=cyan>OnCustomAuthenticationFailed callback was executed.</color>");
    //}
}
