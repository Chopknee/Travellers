using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardVoid : Inventory {


    // Update is called once per frame
    void Update() {
        if (GetComponentInChildren<UICard>() != null) {
            Destroy(GetComponentInChildren<UICard>().gameObject);
            //Spawn a new random card.
        }
    }
}
