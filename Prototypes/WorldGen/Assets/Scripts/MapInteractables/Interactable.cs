using BaD.Modules.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BaD.Modules.Terrain {

    public class Interactable: MonoBehaviour {

        public GameObject HoverInfoGUIPrefab;
        private GameObject HoverInfoGUIInstance;

        public float activationRadius;
        private float activationRadiusSquared;

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

        public void OnInteract ( InputAction.CallbackContext contex ) {
            GameObject playerInst = DungeonManager.CurrentInstance.LocalDungeonPlayerInstance;
            if (( playerInst.transform.position - navigationPoint.position ).sqrMagnitude < activationRadiusSquared) {
                //Activate this thing.
                DoInteraction();
            }
        }

        public virtual void DoInteraction () {
            Debug.Log("Something is using the generic interactable class. This is not good.");
        }

        public virtual string GetShopName () {
            return "Error";
        }

        private void OnTriggerEnter ( Collider other ) {
            if (other.gameObject.CompareTag("Player")) {
                if (HoverInfoGUIInstance != null) {
                    HoverInfoGUIInstance.SetActive(true);
                    HoverInfoGUIInstance.transform.Find("txtShopName").GetComponent<Text>().text = GetShopName();
                }
            }
        }

        private void OnTriggerExit ( Collider other ) {
            if (other.gameObject.CompareTag("Player")) {
                if (HoverInfoGUIInstance != null) {
                    HoverInfoGUIInstance.SetActive(false);
                }
            }
        }

        public void OnEnable () {
            MainControl.Instance.Controls.Player.Interact.performed += OnInteract;
        }

        public void OnDisable () {
            MainControl.Instance.Controls.Player.Interact.performed -= OnInteract;
            if (HoverInfoGUIInstance != null) {
                HoverInfoGUIInstance.SetActive(false);
            }
        }

        public void OnDrawGizmosSelected () {
            if (navigationPoint != null) {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(navigationPoint.position, activationRadius);
            }
        }
    }
}
