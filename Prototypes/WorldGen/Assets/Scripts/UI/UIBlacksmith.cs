using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBlacksmith: MonoBehaviour {
    [SerializeField]
#pragma warning disable 0649
    Recipe[] recipies;

    GameObject activeGUIPanel;

    [SerializeField]
    Transform recipieGUIParent;

    public Color HighlightedColor = Color.green;
    public Color NormalColor = Color.gray;

    void Start () {
        foreach (Recipe r in recipies) {
            r.activatorButton.OnClicked += OnRecipieClicked;
        }
    }

    public void OnRecipieClicked(ButtonFixer bf) {
        foreach (Recipe r in recipies) {
            if (bf == r.activatorButton) {
                if (activeGUIPanel != null) {
                    Destroy(activeGUIPanel);
                }

                activeGUIPanel = Instantiate(r.guiPrefab);
                activeGUIPanel.transform.SetParent(recipieGUIParent);
                activeGUIPanel.transform.localPosition = Vector3.zero;

                DimButtons();
                bf.GetComponent<Image>().color = HighlightedColor;
                break;
            }
        } 
    }

    public void DimButtons() {
        foreach (Recipe r in recipies) {
            r.activatorButton.gameObject.GetComponent<Image>().color = NormalColor;
        }
    }
}

[Serializable]
struct Recipe {
    public ButtonFixer activatorButton;
    public GameObject guiPrefab;
}
