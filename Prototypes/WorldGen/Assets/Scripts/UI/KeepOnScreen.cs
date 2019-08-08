using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This gives whatever ui object it is placed on the behavior of staying on screen at all times.
public class KeepOnScreen : MonoBehaviour {

    RectTransform myRect;
    public Vector3 originalPosition;

    RectTransform canvasRect;

    RectTransform offsetTransform;

    void Awake() {
        myRect = GetComponent<RectTransform>();
        offsetTransform = (RectTransform)myRect.parent;
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        originalPosition = myRect.localPosition;
    }

    void OnEnable() {
        myRect.localPosition = originalPosition;
    }

    public Vector2 minimum;
    public Vector2 maximum;

    public Vector2 pos;
    
    void Update() {
        if (canvasRect != null) {
            myRect.SetParent(canvasRect);
            //Set the 'local space' relative to the main canvas to perform these operations.

            minimum = ( canvasRect.sizeDelta - myRect.sizeDelta ) * -0.5f;
            maximum = ( canvasRect.sizeDelta - myRect.sizeDelta ) * 0.5f;
            pos = myRect.localPosition;
            myRect.localPosition = ClampVector2(myRect.localPosition, minimum, maximum);

            //Set the local space back to the mouse offset transform
            myRect.SetParent(offsetTransform);
        }
    }

    Vector2 ClampVector2 ( Vector2 a, Vector2 min, Vector2 max ) {
        return new Vector2(Mathf.Clamp(a.x, min.x, max.x), Mathf.Clamp(a.y, min.y, max.y));
    }
}
