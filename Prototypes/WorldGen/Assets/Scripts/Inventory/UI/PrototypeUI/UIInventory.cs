using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour {


    UIInventorySlot[] slots;
    Inventory myInventory;


    // Start is called before the first frame update
    void Start() {
        slots = GetComponentsInChildren<UIInventorySlot>();
        myInventory = GetComponent<Inventory>();//Assuming this is attatched to an inventory script
        foreach (UIInventorySlot ui in slots) {
            //Assign the appropriate events.

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSlotDroppedOn() {

    }

    public void OnSlotPickedUpFrom() {

    }
}
