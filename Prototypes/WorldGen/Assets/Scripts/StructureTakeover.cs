using BaD.Modules;
using BaD.Modules.Terrain.Modifiers;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureTakeover : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField]
    //private Structure structureSpawnData;
    void Start() {
        //if (!PhotonNetwork.IsMasterClient) {
        //    //This is only needed because of the way I built map interaction, yay me.
        //    if (structureSpawnData != null && OverworldControl.Instance != null) {
        //        Map map = OverworldControl.Instance.Map;
        //        Vector2 gridPosition = map.RealWorldToTerrainCoord(transform.position);
        //        Tile[,] tilesUnderStructure = map.GetTilesFromRadius(gridPosition, structureSpawnData.radius);
        //        foreach (Tile t in tilesUnderStructure) {
        //            if (t == null)
        //                continue;
        //            t.occupyingObject = gameObject;
        //        }
        //    }
        //}
    }
}
