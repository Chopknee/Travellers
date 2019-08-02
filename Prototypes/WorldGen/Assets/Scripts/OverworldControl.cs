using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class OverworldControl: MonoBehaviour {
    
    public static OverworldControl Instance {
        get {
            return inst;
        }
    }

    private static OverworldControl inst;
    
    private Map map;
    public Map Map {
        get {
            return map;
        }
    }

    public int NoiseSeed {
        get {
            return map.noiseData.seed;
        }
    }

    private void Awake () {
        inst = this;
        map = GetComponent<Map>();
        bp = Instantiate(buildingsPointerPrefab);
        bp.SetActive(false);

    }

    public void Setup() {
        map.Generate();
    }

    public bool GUIOpen {
        get {
            //Check each gui to see if it is in the open state.
            return UIShopTrade.Instance.gameObject.activeSelf;
        }
    }

    public GameObject buildingsPointerPrefab;

    public GameObject BuildingPointer {
        get {
            return bp;
        }
    }

    public GameObject bp;
}
