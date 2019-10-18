using BaD.Modules;
using BaD.Modules.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion: MonoBehaviour {

    public AnimationCurve zoomCurve;
    public float zoomSensitivity = -3;
    public float zoomPercent = 0.5f;
    float zoomSpeed;
    public Vector2 heightConstraintRange = new Vector2(0, 8.9f);
    public Vector2 distanceConstraint = new Vector2(5, 6);

    public float zoomDrag = 4.15f;

    public Transform target;

    public float horizontalSensitivity = 0.15f;
    public float horizontalSmoothing = 10;
    public float horizontal = 0;//In radians

    public float verticalSensitivity = 0.15f;
    public float verticalSmoothing = 10f;
    public float vertical = 0.5f;//between 0 and 1
    public float heightZoomScalar = 2;

    public float smoothTime = 0.1f;

    MainControls mc;

    void Start () {
        verticalSmoothed = vertical;
        mc = MainControl.Instance.Controls;
    }

    readonly float PI2 = Mathf.PI * 2;

    float verticalSmoothed;
    float horizontalSmoothed;

    void LateUpdate () {
        if (target == null) { return; }
        //Vector2 cameraAxis = Pausemenu.InputMasterController.Hammy.C

        int vDir = 1;//( Pausemenu.VerticalInverted ) ? -1 : 1;
        float verticalDelta = mc.Player.CameraVertical.ReadValue<float>() * Time.deltaTime * verticalSensitivity * vDir;
        vertical += verticalDelta;
        vertical = Mathf.Clamp(vertical, 0, 1);
        verticalSmoothed += ( vertical - verticalSmoothed ) * Time.deltaTime * verticalSmoothing;

        int hDir = 1;//( Pausemenu.HorizontalInverted ) ? -1 : 1;
        float horizontalDelta = mc.Player.CameraHorizontal.ReadValue<float>() * Time.deltaTime * horizontalSensitivity * hDir;
        horizontal += horizontalDelta;
        horizontalSmoothed += ( horizontal - horizontalSmoothed ) * Time.deltaTime * horizontalSmoothing;
        // Keeping rotation within the 0 to 2 * PI range
        if (horizontal >= PI2) {
            horizontal -= PI2;
            horizontalSmoothed -= PI2;
        } else if (horizontal < 0) {
            horizontal += PI2;
            horizontalSmoothed += PI2;
        }


        //Update speed (scroll wheel)
        zoomSpeed += mc.Player.CameraZoom.ReadValue<float>() * zoomSensitivity * Time.deltaTime;
        zoomSpeed += -zoomSpeed * zoomDrag * Time.deltaTime;
        //Update the zoom amount
        zoomPercent = Mathf.Clamp01(zoomPercent + zoomSpeed);
        //Zero out values that are extremely low
        if (zoomSpeed > -0.001f && zoomSpeed < 0.001f) {
            zoomSpeed = 0;
        }

        //Calculate the position!!
        transform.position = target.position;
        Vector3 pos = target.position;
        pos.y += Mathf.Lerp(heightConstraintRange.x, heightConstraintRange.y + ( heightZoomScalar * zoomPercent ), verticalSmoothed);
        float dist = Mathf.Lerp(distanceConstraint.x, distanceConstraint.y, zoomCurve.Evaluate(zoomPercent));

        pos.x += Mathf.Sin(horizontalSmoothed) * dist;
        pos.z += Mathf.Cos(horizontalSmoothed) * dist;

        //Point toward the object
        transform.position = pos;
        transform.forward = ( target.position - transform.position ).normalized;
    }


    //Just for the editor
    private void OnValidate () {
        if (!Application.isPlaying) {
            //Height constraint x is the min, must be above 0.
            heightConstraintRange.x = Mathf.Max(0, heightConstraintRange.x);
            //Height constrain y is the max, it must be above the min
            heightConstraintRange.y = Mathf.Max(heightConstraintRange.x + 0.1f, heightConstraintRange.y);

            distanceConstraint.x = Mathf.Max(0, distanceConstraint.x);
            distanceConstraint.y = Mathf.Max(distanceConstraint.x + 0.1f, distanceConstraint.y);
        }
    }
}
