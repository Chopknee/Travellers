using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandFollow : MonoBehaviour
{
    public Transform leftHand, rightHand;
    public enum hands
    {
        left, right
    }
    public hands hand = hands.right;

    public Vector3 rotationOffset = new Vector3(-90, 0, 0),
                   positionOffset = new Vector3(0,0,0);

    // Start is called before the first frame update
    void Start()
    {
        if (leftHand == null)
        {
            leftHand = transform.Find("mixamorig:LeftHand");
        }
        if (rightHand == null)
        {
            rightHand = transform.Find("mixamorig:RightHand");
        }
        transform.position = rightHand.position + positionOffset;
        Vector3 er = rightHand.rotation.eulerAngles + rotationOffset;
        transform.rotation = Quaternion.Euler(er);
        transform.parent = rightHand;
    }
    
}
