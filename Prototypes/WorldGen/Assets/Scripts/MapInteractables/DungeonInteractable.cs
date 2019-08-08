using BaD.Modules;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaD.Modules.Terrain {
    public class DungeonInteractable: MonoBehaviour, IMapInteractable {

        public GameObject pointer;//If not assigned, 
                                  //public Vector3 pointerLocation = new Vector3();
        public Transform pointerLocation;

        public DungeonData instanceInformation;

        // Start is called before the first frame update
        void Start () {
            if (OverworldControl.Instance.BuildingPointer != null && pointer == null) {
                pointer = OverworldControl.Instance.BuildingPointer;
            }
        }

        public string GetActionName () {
            return "Explore " + instanceInformation.Test;
        }

        public string GetDisplayName () {
            return "Ass";
        }

        public string GetShortActionName () {
            return "Enter";
        }

        public void Interact ( Player player ) {
            Debug.Log("You have entered a theoretical dungeon instance, congratulations.");
        }

        public bool InteractionComplete ( Player player ) {
            return true;
        }

        public void SetHighlight ( bool state ) {
            if (state) {
                pointer.transform.position = pointerLocation.transform.position;
                pointer.SetActive(true);
            } else {
                //Prevents this call from overriding previous calls to hide the pointer
                if (pointer.transform.position == pointerLocation.transform.position) {
                    pointer.SetActive(false);
                }
            }
        }

        public InteractResult TryInteract ( Player player ) {
            return new InteractResult(true);
        }
    }
}
