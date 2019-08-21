using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaD.UI.MainMenu {

    public class UIMainMenu: MonoBehaviour {

        [SerializeField]
#pragma warning disable 0649
        private Button SoloButton;
        [SerializeField]
#pragma warning disable 0649
        private Button MultiplayerButton;
        [SerializeField]
#pragma warning disable 0649
        private Button OptionsButton;
        [SerializeField]
#pragma warning disable 0649
        private Button ExitButton;

        [SerializeField]
#pragma warning disable 0649
        private CanvasGroup SoloPlayGroup;
        [SerializeField]
#pragma warning disable 0649
        private CanvasGroup MultiPlayGroup;
        [SerializeField]
#pragma warning disable 0649
        private CanvasGroup OptionsGroup;

        private CanvasGroup myGroup;

        void Start () {

            SoloButton.onClick.AddListener(OnSoloClicked);
            MultiplayerButton.onClick.AddListener(OnMultiplayerClicked);
            OptionsButton.onClick.AddListener(OnOptionsClicked);
            ExitButton.onClick.AddListener(OnExitClicked);
            SoloPlayGroup.GetComponent<UISoloPlayPanel>().OnReturnClicked += OnReturnFromSolo;
            MultiPlayGroup.GetComponent<UIMultiplayPanel>().OnReturnClicked += OnReturnFromMultiplay;

            myGroup = GetComponent<CanvasGroup>();


        }

        private void OnSoloClicked() {
            SetGroupActive(myGroup, false);
            SetGroupActive(SoloPlayGroup, true);
        }

        private void OnReturnFromSolo() {
            SetGroupActive(myGroup, true);
            SetGroupActive(SoloPlayGroup, false);
        }

        private void OnMultiplayerClicked() {
            SetGroupActive(MultiPlayGroup, true);
            SetGroupActive(myGroup, false);
        }

        private void OnReturnFromMultiplay() {
            SetGroupActive(MultiPlayGroup, false);
            SetGroupActive(myGroup, true);
        }

        private void OnOptionsClicked() {
            SetGroupActive(OptionsGroup, true);
            SetGroupActive(myGroup, false);
        }

        private void OnExitClicked() {
            Application.Quit();
        }

        private void SetGroupActive(CanvasGroup group, bool active) {
            if (active) {
                group.alpha = 1;
                group.interactable = true;
                group.blocksRaycasts = true;
            } else {
                group.alpha = 0;
                group.interactable = false;
                group.blocksRaycasts = false;
            }
        }
    }
}
