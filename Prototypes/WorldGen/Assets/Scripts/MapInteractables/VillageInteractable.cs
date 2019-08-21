using BaD.Modules;
using BaD.Modules.Terrain;
using UnityEngine;

public class VillageInteractable : MapInteractable {

    public GameObject VillageInstanceManager;
    public bool returnToOverworld = false;
    private GameObject villageInstance;
    private DungeonManager dm;
    public void Start () {
        villageInstance = Instantiate(VillageInstanceManager);
    }

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

        int instanceSeed = Mathf.RoundToInt(transform.position.x + transform.position.y + transform.position.z);//Seed based on position?
        dm = villageInstance.GetComponent<DungeonManager>();
        dm.GeneratorSeed = instanceSeed;
        dm.EnterInstance();
        returnToOverworld = false;
    }

    public override bool InteractionComplete ( Player player ) {
        return !dm.Showing;
    }


    public void OnMouseEnter () {
        //Show a highlight?
        SetHighlight(true);
    }

    public void OnMouseExit () {
        //Hide a highlight?
        SetHighlight(true);
    }
}
