using BaD.Chopknee.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITargetObject : MonoBehaviour {

    private RectTransform myRect;
    private RectTransform canvasRect;
    public Transform target;

    private void Start () {
        myRect = GetComponent<RectTransform>();
        canvasRect = GetComponentInParent<RectTransform>();
    }

    void Update() {
        if (target != null) {
            Vector3 worldPos = Choptilities.WorldToScreenPointProjected(Camera.main, target.position);
            worldPos = new Vector3(worldPos.x, worldPos.y, 0);
            Vector2 minimum, maximum;
            minimum = ( canvasRect.sizeDelta - myRect.sizeDelta ) * -0.5f;
            maximum = ( canvasRect.sizeDelta - myRect.sizeDelta ) * 0.5f;
            myRect.position = worldPos;
        }
    }

    private void OnTransformParentChanged () {
        canvasRect = GetComponentInParent<RectTransform>();
    }
}
