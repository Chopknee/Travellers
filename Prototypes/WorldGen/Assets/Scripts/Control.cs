using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class Control: MonoBehaviour {
    
    public static Control Instance {
        get {
            return inst;
        }
    }

    private static Control inst;
    
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
    }

    void Start () {
        map.Generate();
    }

    void Update() {
        
    }
}
