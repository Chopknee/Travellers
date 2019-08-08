using BaD.Modules.Terrain;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaD.Modules.Networking {
    public class Launcher: MonoBehaviourPunCallbacks {
        string gameVersion = "0.1";

        [SerializeField]
#pragma warning disable 0649
        private Button multiplayerButton;
        [SerializeField]
#pragma warning disable 0649
        private Button nickEnteredButton;
        [SerializeField]
#pragma warning disable 0649
        private Button newGameButton;
        [SerializeField]
#pragma warning disable 0649
        private Button connectToGameButton;
        [SerializeField]
#pragma warning disable 0649
        private byte maxPlayersPerRoom = 4;//For now I suppose??
        [SerializeField]
#pragma warning disable 0649
        public Text informationMessages;

        [SerializeField]
#pragma warning disable 0649
        private RectTransform MultiplayerPanel;
        [SerializeField]
#pragma warning disable 0649
        private RectTransform NickPanel;
        [SerializeField]
#pragma warning disable 0649
        private RectTransform ConnectOrJoinPanel;

        public void Awake () {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public void Start () {
            //Registering the map data to be sent... yay, so much pain in this single line.

            newGameButton.onClick.AddListener(NewGameClicked);
            connectToGameButton.onClick.AddListener(ConnectClicked);
            multiplayerButton.onClick.AddListener(MultiplayerClicked);
            nickEnteredButton.onClick.AddListener(NickEnteredClicked);

        }

        public void MultiplayerClicked() {
            MultiplayerPanel.gameObject.SetActive(false);
            NickPanel.gameObject.SetActive(true);
        }

        public void NickEnteredClicked() {
            if (!HasName()) {
                LogMessage("Enter a username before connecting.", true);
                return;
            }
            LogMessage("Connecting to photon servers...", false);
            nickEnteredButton.interactable = false;
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
            Invoke("OnConnectToMasterFailed", 15);
        }

        public override void OnConnectedToMaster () {
            //We connected, now the next window can be displayed
            LogMessage("Successfully connected to master.", false);
            NickPanel.gameObject.SetActive(false);
            ConnectOrJoinPanel.gameObject.SetActive(true);
            nickEnteredButton.interactable = true;
        }

        public void OnConnectToMasterFailed() {
            if (PhotonNetwork.IsConnected)
                return;
            nickEnteredButton.interactable = true;
            LogMessage("Connection to master server has timed out.", true);
        }

        public void NewGameClicked () {
            LogMessage("Attempting to create new room...", false);
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
            connectToGameButton.interactable = false;
            newGameButton.interactable = false;
        }

        public override void OnCreatedRoom () {
            LogMessage("Room was successfully created, joining now...", false);
            JoinRoom();
        }

        public override void OnCreateRoomFailed ( short returnCode, string message ) {
            connectToGameButton.interactable = true;
            newGameButton.interactable = true;
            LogMessage("Failed to create room. Code " + returnCode + ": " + message, true);
        }

        public void ConnectClicked () {
            connectToGameButton.interactable = false;
            newGameButton.interactable = false;
            JoinRoom();
        }

        public void JoinRoom () {
            LogMessage("Joining existing room...", false);
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinedRoom () {
            LogMessage("Room joined, loading game...", false);
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
                LogMessage("No current players, loading new instance of the room...", false);
                PhotonNetwork.LoadLevel("Game");
            }
        }

        public override void OnJoinRandomFailed ( short returnCode, string message ) {
            connectToGameButton.interactable = true;
            newGameButton.interactable = true;
            LogMessage("Failed to create room. Code " + returnCode + ": " + message, true);
        }

        public override void OnDisconnected ( DisconnectCause cause ) {
            MultiplayerPanel.gameObject.SetActive(true);
            NickPanel.gameObject.SetActive(false);
            nickEnteredButton.interactable = true;
            ConnectOrJoinPanel.gameObject.SetActive(false);
            connectToGameButton.interactable = true;
            newGameButton.interactable = true;
            LogMessage("Disconnected from room. Cause: " + cause, true);
        }

        private void LogMessage ( string message, bool isError ) {
            informationMessages.text = message;
            if (isError) {
                Debug.Log("<color=red>" + message + "</color>");
                informationMessages.color = Color.red;
            } else {
                Debug.Log(message);
                informationMessages.color = Color.black;
            }
        }

        public bool HasName() {
            return PhotonNetwork.NickName != "";
        }
    }
}