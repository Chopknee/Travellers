using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCHealth : MonoBehaviour
{
    Health h;
    private void Awake()
    {
        h = GetComponent<Health>();
    }


    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4)) // 1 is remove 15 health immediately.
        {
            h.ChangeHealth(false, 100f, true, 2f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5)) // 2 is remove 15 health over time at a rate of .15f.
        {
            h.ChangeHealth(false, 600f, true, 4f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6)) // 3 is remove 50 health immediately.
        {
            h.ChangeHealth(false, 50f, false, 1f);
        }
    }
    
    
}
