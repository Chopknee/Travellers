using BaD.Chopknee.Utilities;
using BaD.Modules;
using BaD.Modules.Control;
using BaD.Modules.Networking;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaD.Modules.Terrain {
    public class BlacksmithInteractable: MonoBehaviour {

        public GameObject HoverInfoGUIPrefab;
        private GameObject HoverInfoGUIInstance;

        public GameObject BlacksmithGUIPrefab;
        private GameObject BlacksmithGUIInstance;
        
        bool isHighlighted;

        public float activationRadius;
        private float activationRadiusSquared;

        bool isCurrentNavTarget = false;

        int inventoryID;

        public Transform navigationPoint;

        public void Start () {
            if (navigationPoint == null) {
                navigationPoint = transform;
            }

            activationRadiusSquared = activationRadius * activationRadius;

            if (HoverInfoGUIPrefab != null) {
                HoverInfoGUIInstance = Instantiate(HoverInfoGUIPrefab);
                HoverInfoGUIInstance.GetComponent<UITargetObject>().target = transform;
                HoverInfoGUIInstance.SetActive(false);
                HoverInfoGUIInstance.transform.SetParent(MainControl.Instance.ActionConfirmationUI.transform);
            }
        }

        public void Update () {
            if (Input.GetButtonDown("Interact")) {
                if (!isHighlighted) {
                    if (isCurrentNavTarget) {
                        isCurrentNavTarget = false;
                    }
                } else {
                    //Do the appropriate stuff here.
                    GameObject playerInst = DungeonManager.CurrentInstance.localPlayerInstance;
                    if (( playerInst.transform.position - navigationPoint.position ).sqrMagnitude < activationRadiusSquared) {
                        //Activate this thing.
                        OpenBlacksmith();
                    } else {
                        isCurrentNavTarget = true;
                        playerInst.GetComponent<PlayerMovement>().SetDestination(navigationPoint.position, true);

                        //Move to this thing, then activate it.
                    }
                }
            }

            if (isCurrentNavTarget) {
                GameObject playerInst = DungeonManager.CurrentInstance.localPlayerInstance;
                if (( playerInst.transform.position - transform.position ).sqrMagnitude < activationRadiusSquared) {
                    isCurrentNavTarget = false;
                    OpenBlacksmith();
                }
            }
        }

        public void OnMouseDown () {

        }

        private void OpenBlacksmith() {
            BlacksmithGUIInstance = Instantiate(BlacksmithGUIPrefab);
            BlacksmithGUIInstance.GetComponent<UIWindow>().OnClosed += CloseBlacksmith;
        }

        private void CloseBlacksmith() {
            //Runs when the blacksmith window is closed...
        }

        public void OnMouseEnter () {
            if (HoverInfoGUIInstance != null) {
                HoverInfoGUIInstance.SetActive(true);
                HoverInfoGUIInstance.transform.Find("txtShopName").GetComponent<Text>().text = "Blacksmith";
                isHighlighted = true;
            }
        }

        public void OnMouseExit () {
            if (HoverInfoGUIInstance != null) {
                HoverInfoGUIInstance.SetActive(false);
                isHighlighted = false;
            }
        }

        public void OnDisable () {
            if (HoverInfoGUIInstance != null) {
                HoverInfoGUIInstance.SetActive(false);
                isHighlighted = false;
            }
        }

        public void OnDrawGizmos () {
            if (navigationPoint != null) {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(navigationPoint.position, activationRadius);
            }
        }
    }
}