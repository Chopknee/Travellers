using BaD.Modules;
using BaD.Modules.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlacksmithRecipe: MonoBehaviour {

    [SerializeField]
#pragma warning disable 0649
    RecipieItem[] recipeItems;

    [SerializeField]
#pragma warning disable 0649
    TextMeshProUGUI descriptionLabel;

    [SerializeField]
#pragma warning disable 0649
    Button turnInButton;

    [SerializeField]
#pragma warning disable 0649
    Color buttonHighlightColor;

    [SerializeField]
#pragma warning disable 0649
    Color buttonRegularColor;

    bool canTurnIn = false;

    void Start () {
        Setup();
        CheckForValidConditions();
        EventSystem.current.SetSelectedGameObject(recipeItems[0].clickableButton.gameObject);
    }

    void Setup() {
        
        //Add onclick listeners to each button.
        foreach (RecipieItem ri in recipeItems) {
            ri.clickableButton.OnClicked += RecipeButtonClicked;
        }
        

        //Add listener to turn in button.
        turnInButton.onClick.AddListener(OnTurnInClicked);
    }

    void UnSetup() {
        //Add onclick listeners to each button.
        foreach (RecipieItem ri in recipeItems) {
            ri.clickableButton.OnClicked -= RecipeButtonClicked;
        }

        //Add listener to turn in button.
        turnInButton.onClick.RemoveListener(OnTurnInClicked);
    }

    void CheckForValidConditions() {
        canTurnIn = false;
        //Highlight in green the items that are fulfilled.
        if (MainControl.Instance.LocalPlayerData.Inventory.Items.Length > 0) {
            foreach (ItemInstance it in MainControl.Instance.LocalPlayerData.Inventory.Items) {
                foreach (RecipieItem ri in recipeItems) {
                    if (ri.isInput) {
                        if (NetworkedInventoryManager.Instance.Compare(it, ri.item)) {
                            ri.clickableButton.GetComponent<Image>().color = buttonHighlightColor;
                            canTurnIn &= true;
                            break;
                        } else {
                            ri.clickableButton.GetComponent<Image>().color = buttonRegularColor;
                            canTurnIn &= false;
                            break;
                        }
                    }
                }
            }
        } else {
            foreach (RecipieItem ri in recipeItems) {
                if (ri.isInput) {
                    ri.clickableButton.GetComponent<Image>().color = buttonRegularColor;
                }
            }
        }
    }

    void RecipeButtonClicked(ButtonFixer b ) {
        foreach(RecipieItem ri in recipeItems) {
            if (b == ri.clickableButton) {
                descriptionLabel.text = ri.richText;
                break;
            }
        }
    }

    void OnTurnInClicked() {
        if (canTurnIn) {
            //Since th
            //Remove the appropriate items from the player's inventory
            foreach (RecipieItem ri in recipeItems) {
                if (ri.isInput) {
                    MainControl.Instance.LocalPlayerData.Inventory.RemoveItem(ri.item);
                }
            }

            CheckForValidConditions();
        }
        //Check if the player is holding all the required items.
    }
}

[Serializable]
struct RecipieItem {
    public ItemType item;
    public ButtonFixer clickableButton;
    [Multiline]
    public string richText;
    public bool isInput;
}
