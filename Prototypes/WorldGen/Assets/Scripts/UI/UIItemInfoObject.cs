using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ItemType;
namespace BaD.UI.DumpA {
    public class UIItemInfoObject: MonoBehaviour {

        public ItemType ItemData {
            get {
                return cd;
            }
            set {
                cd = value;
                GenerateCard();
            }
        }
        private ItemType cd;

        [Header("UI Components")]
        [SerializeField]
#pragma warning disable 0649
        private Text descriptionText;
        [SerializeField]
#pragma warning disable 0649
        private Text nameText;
        [SerializeField]
#pragma warning disable 0649
        private Text valueText;
        [SerializeField]
#pragma warning disable 0649
        private Image itemImage;
        [SerializeField]
#pragma warning disable 0649
        private Image border;
        [SerializeField]
#pragma warning disable 0649
        private Text tierText;
        [SerializeField]
#pragma warning disable 0649
        private GameObject setTokenPrefab;
        [SerializeField]
#pragma warning disable 0649
        private List<GameObject> collectionTokens;
        [SerializeField]
#pragma warning disable 0649
        private GameObject itemBackground;

        void OnValidate () {
            if (ItemData != null) {
                ItemData.OnValuesUpdated -= GenerateCard;
                ItemData.OnValuesUpdated += GenerateCard;

                if (ItemData.collections != null) {
                    foreach (Collection c in ItemData.collections) {
                        c.OnValuesUpdated -= UpdateCollections;
                        c.OnValuesUpdated += UpdateCollections;
                    }
                }

            }

            GenerateCard();

        }

        public void GenerateCard () {
            if (ItemData == null) {
                return;
            }
            if (descriptionText != null) {
                descriptionText.text = ItemData.description;
            }
            if (nameText != null) {
                nameText.text = ItemData.itemName;
            }

            if (valueText != null) {
                valueText.text = string.Format("{0, 0:D1}g", ItemData.value);
            }

            if (itemImage != null) {
                itemImage.sprite = ItemData.itemSprite;
            }

            if (tierText != null) {
                switch (ItemData.cardTier) {
                    case Tier.Tier1:
                        tierText.text = "1";
                        break;
                    case Tier.Tier2:
                        tierText.text = "2";
                        break;
                    case Tier.Tier3:
                        tierText.text = "3";
                        break;
                }
            }

            UpdateCollections();

        }

        public void UpdateCollections () {

            if (ItemData == null || ItemData.collections == null || ItemData.collections.Length == 0) {
                return;
            }

            if (border != null) {
                if (ItemData.collections[0] != null) {
                    border.color = ItemData.collections[0].displayColor;
                } else {
                    border.color = new Color(0, 0, 0, 1);
                }
            }

            foreach (GameObject go in collectionTokens) {
                go.SetActive(false);
            }

            if (itemBackground != null) {
                for (int i = 1; i < ItemData.collections.Length; i++) {
                    if (ItemData.collections[i] != null) {
                        if (i > 9) {
                            break;
                        }
                        GameObject go = collectionTokens[i-1];
                        go.GetComponent<Image>().color = ItemData.collections[i].displayColor;
                        go.SetActive(true);
                    }
                }
            }

        }
    }
}
