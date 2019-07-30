using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainControl : MonoBehaviour {

    public static MainControl Instance {
        get {
            return inst;
        }
    }

    private static MainControl inst;


    public GameObject ShopGUI;
    public GameObject ActionConfirmationGUI;

    public GameObject MapControl;

    void Awake() {
        inst = this;
        if (ShopGUI == null) {
            Debug.Log("Shop GUI has not been assigned to Main Control!");
        }

        if (ActionConfirmationGUI == null) {
            Debug.Log("Action Confirmation GUI has not been assigned to Main Control!");
        }

        if (MapControl == null) {
            Debug.Log("Map Control has not been assigned to Main Control!");
        }

    }

    void Start() {
        //Probably temporary but works for now.
        MapControl.GetComponent<OverworldControl>().Setup();
    }
}
