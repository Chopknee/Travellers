using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 dir = transform.position - Camera.main.transform.position;
        dir = new Vector3(dir.x, 0, dir.z);
        transform.rotation = Quaternion.LookRotation(dir);
        
    }
}
