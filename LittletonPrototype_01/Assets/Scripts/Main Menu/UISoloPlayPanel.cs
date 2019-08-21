using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaD.UI.MainMenu {

    public class UISoloPlayPanel: MonoBehaviour {

        public delegate void Returned ();
        public Returned OnReturnClicked;

        [SerializeField]
#pragma warning disable 0649
        private InputField MapSeedInput;
    
        [SerializeField]
#pragma warning disable 0649
        private Button StartButton;
        [SerializeField]
#pragma warning disable 0649
        private Button BackButton;

        // Start is called before the first frame update
        void Start () {
            BackButton.onClick.AddListener(BackButtonClicked);
            StartButton.onClick.AddListener(StartButtonClicked);
        }

        void BackButtonClicked() {
            OnReturnClicked?.Invoke();
        }

        void StartButtonClicked() {
            Debug.Log("Need to start the game with seed number of " + MapSeedInput.text);
        }
    }

}