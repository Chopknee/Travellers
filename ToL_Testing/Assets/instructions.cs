using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class instructions : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey) { gameObject.SetActive(false); }
    }
}
