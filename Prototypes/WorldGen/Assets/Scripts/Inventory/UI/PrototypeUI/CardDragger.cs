using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDragger : MonoBehaviour {

    public static CardDragger Instance {
        get {
            return inst;
        }
    }

    private static CardDragger inst;
    // Start is called before the first frame update
    void Start() {
        inst = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
