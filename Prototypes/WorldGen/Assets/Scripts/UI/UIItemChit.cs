using BaD.Modules.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BaD.UI.DumpA {
    public class UIItemChit: MonoBehaviour, ISelectHandler, IPointerEnterHandler {

        public ItemType ItemData {
            get {
                return cd;
            }
            set {
                CardDataChanged(value);
                cd = value;
            }
        }

        private ItemType cd;

        public ItemInstance instance;

        [SerializeField]
#pragma warning disable 0649
        private Image itemImage;

        public delegate void Clicked ( UIItemChit me);
        public Clicked OnClicked;
        public delegate void Highlighted ( UIItemChit me );
        public Highlighted OnHighlighted;

        public void Start () {
            GetComponent<Button>().onClick.AddListener(IGotClicked);   
        }

        public void IGotClicked() {
            OnClicked?.Invoke(this);
        }

        void CardDataChanged( ItemType newCard ) {
            itemImage.sprite = newCard.itemSprite;
        }

        public void OnSelect ( BaseEventData eventData ) {
            OnHighlighted?.Invoke(this);
        }

        public void OnPointerEnter ( PointerEventData eventData ) {
            OnHighlighted?.Invoke(this);
        }
    }
}