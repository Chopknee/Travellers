﻿using BaD.Modules;
using BaD.Modules.Terrain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BaD.UI.DumpA {
    public class UIPlayerHUD: MonoBehaviour {

        [SerializeField]
#pragma warning disable 0649
        private Button MenuButton;
        [SerializeField]
#pragma warning disable 0649
        private Button InventoryButton;
        [SerializeField]
#pragma warning disable 0649
        private TextMeshProUGUI PlayerNameText;
        [SerializeField]
#pragma warning disable 0649
        private TextMeshProUGUI PlayerGoldText;

        public PlayerData associatedPlayer;

        // Start is called before the first frame update
        void Start () {
            //I can't rely on the player object existing when things first start up.
        }

        // Update is called once per frame
        void Update () {
            if (associatedPlayer == null) {
                if (MainControl.LocalPlayerData != null) {
                    //Get the player data from the player object
                    associatedPlayer = MainControl.LocalPlayerData;
                    if (associatedPlayer != null) {
                        associatedPlayer.OnPlayerGoldChanged += OnGoldChanged;
                        associatedPlayer.OnPlayernameChanged += OnNameChanged;
                        OnGoldChanged(associatedPlayer);
                        OnNameChanged(associatedPlayer);
                        MenuButton.onClick.AddListener(OnMenuButtonClicked);
                        InventoryButton.onClick.AddListener(OnInventoryButtonClicked);
                        OnNameChanged(associatedPlayer);
                    }
                }
            }
        }

        void OnGoldChanged(PlayerData player) {
            PlayerGoldText.text = string.Format("{0, 0:D3}g", player.gold);
        }

        void OnNameChanged(PlayerData player) {
            PlayerNameText.text = player.Name;
        }

        void OnMenuButtonClicked() {
            Debug.Log("Menu Button Clicked!");
        }

        void OnInventoryButtonClicked() {
            MainControl.Instance.PlayerInventoryInstance.Open(associatedPlayer.Inventory);
        }
    }
}
