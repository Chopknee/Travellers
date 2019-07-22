using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ItemCard;

public class ItemCardObject: MonoBehaviour {

    public ItemCard CardData {
        get {
            return cd;
        }
        set {
            cd = value;
            GenerateCard();
        }
    }
    private ItemCard cd;

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
        if (CardData != null) {
            CardData.OnValuesUpdated -= GenerateCard;
            CardData.OnValuesUpdated += GenerateCard;

            if (CardData.collections != null) {
                foreach (Collection c in CardData.collections) {
                    c.OnValuesUpdated -= UpdateCollections;
                    c.OnValuesUpdated += UpdateCollections;
                }
            }

        }

        GenerateCard();

    }

    public void GenerateCard() {
        if (CardData == null) {
            return;
        }
        if (descriptionText != null) {
            descriptionText.text = CardData.description;
        }
        if (nameText != null) {
            nameText.text = CardData.itemName;
        }

        if (valueText != null) {
            valueText.text = string.Format("{0, 0:D1}g", CardData.value);
        }

        if (itemImage != null) {
            itemImage.sprite = CardData.itemSprite;
        }

        if (tierText != null) {
            switch (CardData.cardTier) {
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

        if (CardData == null || CardData.collections == null || CardData.collections.Length == 0) {
            return;
        }

        if (border != null) {
            if (CardData.collections[0] != null) {
                border.color = CardData.collections[0].displayColor;
            } else {
                border.color = new Color(0,0,0,1);
            }
        }

        foreach (GameObject go in collectionTokens) {
            go.SetActive(false);
        }

        if (cardBackground != null) {
            for (int i = 1; i < CardData.collections.Length; i++) {
                if (CardData.collections[i] != null) {
                    if (i > 9) {
                        break;
                    }
                    GameObject go = collectionTokens[i - 1];
                    go.GetComponent<Image>().color = CardData.collections[i].displayColor;
                    go.SetActive(true);
                }
            }
        }

    }
}
