using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MouseBehaviour : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            if (hit.transform.CompareTag("GameTile"))
            {
                Debug.Log("Found Game Tile.");
                hit.transform.GetComponent<Material>().SetInt("_glowTrue", 1);
            }
            //else { hit.transform.GetComponent<Material>().SetInt("_glowTrue", 0); }

            // Do something with the object that was hit by the raycast.
        }
    }
}
