using UnityEngine;

namespace BaD.Modules.Terrain {
    public interface IMapInteractable {
        //Defines an object that can be interacted with on the map

        string GetDisplayName ();
        string GetActionName ();
        string GetShortActionName ();
        void Interact ( Player player );//Not sure what to return with this function for now
        InteractResult TryInteract ( Player player );

        void SetHighlight ( bool state );

        bool InteractionComplete ( Player player );

        Vector2 GetClosestPoint ( Player player );

    }

    public class InteractResult {
        //Return some basic information about the interaction
        public bool Interactable { get; private set; }
        private bool interactable = false;

        public enum Reason { None, TooFar, Occupied };

        public Reason FailReason { get; private set; }

        public InteractResult ( bool canInteract, Reason reason = Reason.None ) {
            Interactable = canInteract;
            FailReason = reason;
        }


    }
}