using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradualSettle : MonoBehaviour {

    public float targetRotation;
    public Vector3 rotationAxis = new Vector3(0, 0, 1);
    public AnimationCurve settleCurve;
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    public bool changed = false;
    public float delta;
    public float overshoot = 2;
    public float settleTimeScale = 1;
    private float targ;
    private float orig;
    void Update() {
        if (targ != targetRotation) {
            if (!changed) {
                changed = true;
                delta = 0;
                orig = targ;
                targ = targetRotation;
            }
        }

        if (changed) {
            //Attempt to settle the scales to the set angle?
            //Debug.Log(settleCurve.Evaluate(delta));
            float diff = targ - orig;
            diff = diff * settleCurve.Evaluate(delta);

            transform.localRotation = Quaternion.Euler(rotationAxis * (orig + diff));
            delta += Time.deltaTime * settleTimeScale;
            if (delta >= 1) {
                changed = false;
            }
            
        }
    }

    public void OnMouseDown() {
        targetRotation += 10;
        if (targetRotation > 30) {
            targetRotation = -30;
        }
    }
}
