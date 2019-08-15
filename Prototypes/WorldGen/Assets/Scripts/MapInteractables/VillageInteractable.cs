using BaD.Modules.Terrain;
using UnityEngine;

public class VillageInteractable : MapInteractable {

    public override string GetActionName () {
        return "Enter " + GetDisplayName();
    }

    public override string GetDisplayName () {
        return "Test Village Name";
    }

    public override string GetShortActionName () {
        return "Enter";
    }

    public override void Interact ( Player player ) {
        Debug.Log("Entering village instance!");
    }

    public override bool InteractionComplete ( Player player ) {
        return true;
    }

}
