
public interface IMapInteractable {
    //Defines an object that can be interacted with on the map

    string GetDisplayName();
    string GetActionName();
    string GetShortActionName();
    void Interact(Player player);//Not sure what to return with this function for now
    InteractResult TryInteract(Player player);

    void SetHighlight(bool state);

    bool InteractionComplete(Player player);
    
}

public class InteractResult {
    //Return some basic information about the interaction
    public bool Interactable {
        get {
            return interactable;
        }
    }
    private bool interactable = false;

    public InteractResult(bool canInteract) {
        interactable = canInteract;
    }

    
}
