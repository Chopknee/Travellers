using BaD.Modules;
using BaD.Modules.Terrain;
using BaD.Modules.Terrain.Modifiers;
using UnityEngine;

public abstract class MapInteractable : MonoBehaviour, IMapInteractable {

    Structure spawnData {
        get {
            if (st == null) {
                st = GetComponent<StructureDataLink>().structureData;
            }
            return st;
        }
    }
    Structure st;

    [SerializeField]
#pragma warning disable 0649
    public GameObject pointer;//If not assigned, 
    [SerializeField]
#pragma warning disable 0649
    private Transform pointerLocation;


    public Vector2 GetClosestPoint ( Player player ) {

        float count = ( spawnData.radius + 1 ) * 4f;
        float step = ( Mathf.PI * 4f ) / count;
        float rads = 0;
        Vector2 gp = OverworldControl.Instance.Map.RealWorldToTerrainCoord(transform.position);
        for (int i = 0; i < count; i++) {
            float x = ( spawnData.radius + 1 ) * Mathf.Cos(rads);
            float y = ( spawnData.radius + 1 ) * Mathf.Sin(rads);
            rads += step;
            Tile t = OverworldControl.Instance.Map.tileManager.GetTile(gp + new Vector2(x, y));
            if (t != null) {
                if (!t.Blocked) {
                    return t.gridPosition;
                }
            }
        }
        return Vector3.one * -1;
    }

    public void SetHighlight ( bool state ) {
        if (pointer == null && OverworldControl.Instance.BuildingPointer != null) {
            pointer = OverworldControl.Instance.BuildingPointer;
        }
        //Enable the pointer
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

    public GameObject GetGameObject () {
        return gameObject;
    }

    public abstract string GetActionName ();

    public abstract string GetDisplayName ();
    
    public abstract string GetShortActionName ();

    public abstract void Interact ( Player player );

    public abstract bool InteractionComplete ( Player player );

    public virtual InteractResult TryInteract ( Player player ) {
        Map map = OverworldControl.Instance.Map;
        Vector2 playerGP = map.RealWorldToTerrainCoord(player.transform.position);
        Vector2 shopGP = map.RealWorldToTerrainCoord(transform.position);
        float rad = map.terrainData.uniformScale * spawnData.radius;
        float dist = ( playerGP - shopGP ).sqrMagnitude - ( rad * rad );
        //Check if the player is close enough to interact.
        if (dist <= 0) {
            return new InteractResult(true);
        }
        return new InteractResult(false, InteractResult.Reason.TooFar);
    }
}
