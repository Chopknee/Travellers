using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ItemCard;

public class ItemCardObject: MonoBehaviour {

    public ItemCard cardData;
    //public Collection[] collections;

    [Header("UI Components")]
    public Text descriptionText;
    public Text nameText;
    public Text valueText;
    public Image itemImage;
    public Image border;
    public Text tierText;
    public GameObject setTokenPrefab;
    public List<GameObject> collectionTokens;
    public GameObject cardBackground;

    void Start () {
        collectionTokens = new List<GameObject>();
    }


    void OnValidate () {
        if (cardData != null) {
            cardData.OnValuesUpdated -= GenerateCard;
            cardData.OnValuesUpdated += GenerateCard;

            if (cardData.collections != null) {
                foreach (Collection c in cardData.collections) {
                    c.OnValuesUpdated -= UpdateCollections;
                    c.OnValuesUpdated += UpdateCollections;
                }
            }

        }

        GenerateCard();

    }

    public void GenerateCard() {
        if (descriptionText != null) {
            descriptionText.text = cardData.description;
        }
        if (nameText != null) {
            nameText.text = cardData.itemName;
        }

        if (valueText != null) {
            valueText.text = string.Format("{0, 0:D1}g", cardData.value);
        }

        if (itemImage != null) {
            itemImage.sprite = cardData.itemSprite;
        }

        if (tierText != null) {
            switch (cardData.cardTier) {
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

        if (cardData == null || cardData.collections == null || cardData.collections.Length == 0) {
            return;
        }

        if (border != null) {
            if (cardData.collections[0] != null) {
                    border.color = cardData.collections[0].displayColor;
            } else {
                border.color = new Color(0,0,0,1);
            }
        }

        foreach (GameObject go in collectionTokens) {
            go.SetActive(false);
        }

        if (cardBackground != null) {
            for (int i = 1; i < cardData.collections.Length; i++) {
                if (cardData.collections[i] != null) {
                    if (i > 9) {
                        break;
                    }
                    GameObject go = collectionTokens[i - 1];
                    go.GetComponent<Image>().color = cardData.collections[i].displayColor;
                    go.SetActive(true);
                }
            }
        }

    }
}
