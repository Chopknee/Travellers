using BaD.Modules.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaD.Modules.Terrain {

    public class Interactable: MonoBehaviour {

        public GameObject HoverInfoGUIPrefab;
        private GameObject HoverInfoGUIInstance;

        bool isMouseover;

        public float activationRadius;
        private float activationRadiusSquared;

        public bool isCurrentNavTarget = false;

        public Transform navigationPoint;

        public virtual void Start () {
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
            if (UnityEngine.Input.GetButtonDown("Interact")) {
                if (!isMouseover) {
                    if (isCurrentNavTarget) {
                        isCurrentNavTarget = false;
                    }
                } else {
                    //Do the appropriate stuff here.
                    GameObject playerInst = DungeonManager.CurrentInstance.LocalDungeonPlayerInstance;
                    if (( playerInst.transform.position - navigationPoint.position ).sqrMagnitude < activationRadiusSquared) {
                        //Activate this thing.
                        DoInteraction();
                    } else {
                        isCurrentNavTarget = true;
                        //playerInst.GetComponent<PlayerMovement>().SetDestination(navigationPoint.position, true);
                        //Move to this thing, then activate it.
                    }
                }
            }

            if (isCurrentNavTarget) {
                GameObject playerInst = DungeonManager.CurrentInstance.LocalDungeonPlayerInstance;
                if (( playerInst.transform.position - navigationPoint.position ).sqrMagnitude < activationRadiusSquared) {
                    isCurrentNavTarget = false;
                    DoInteraction();
                }
            }
        }

        public virtual void DoInteraction () {
            Debug.Log("Something is using the generic interactable class. This is not good.");
        }

        public virtual string GetShopName() {
            return "Error";
        }

        public void OnMouseEnter () {
            if (HoverInfoGUIInstance != null) {
                HoverInfoGUIInstance.SetActive(true);
                HoverInfoGUIInstance.transform.Find("txtShopName").GetComponent<Text>().text = GetShopName();
                isMouseover = true;
            }
        }

        public void OnMouseExit () {
            if (HoverInfoGUIInstance != null) {
                HoverInfoGUIInstance.SetActive(false);
                isMouseover = false;
            }
        }

        public void OnDisable () {
            if (HoverInfoGUIInstance != null) {
                HoverInfoGUIInstance.SetActive(false);
                isMouseover = false;
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
