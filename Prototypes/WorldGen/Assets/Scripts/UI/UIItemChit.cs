using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaD.UI.DumpA {
    public class UIItemChit: MonoBehaviour {

        public ItemCard ItemData {
            get {
                return cd;
            }
            set {
                CardDataChanged(value);
                cd = value;
            }
        }

        private ItemCard cd;

        [SerializeField]
#pragma warning disable 0649
        private Image itemImage;

        public delegate void Clicked ( UIItemChit me);
        public Clicked OnClicked;

        public void Start () {
            GetComponent<Button>().onClick.AddListener(IGotClicked);
        }

        public void IGotClicked() {
            OnClicked?.Invoke(this);
        }

        void CardDataChanged( ItemCard newCard ) {
            itemImage.sprite = newCard.itemSprite;
        }
    }
}