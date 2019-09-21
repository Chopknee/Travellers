using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowDragShoot : MonoBehaviour
{

    bool firing;
    Vector3 start, end, mouseBeginPosition, mouseCurrentPosition;

    public LayerMask layer_mask;

    public Transform startT, endT, powerBall, mouseBall;

    float distanceBetweenMousePoints;

    LineRenderer lr;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {

        if (firing)
        {
            start = transform.position;
        }

        mouseCurrentPosition = Input.mousePosition;

        if (Input.GetButtonDown("Attack"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            mouseBeginPosition = Input.mousePosition;
            powerBall.position = start;

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Camera.main.farClipPlane, layer_mask))
            {
                Vector3 directionOfTarget = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                end = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            }
        }
        if (Input.GetButton("Attack"))
        {
            firing = true;

            distanceBetweenMousePoints = Vector3.Distance(mouseBeginPosition, mouseCurrentPosition);

            powerBall.position = Vector3.Lerp(start, end, .5f * distanceBetweenMousePoints * Time.fixedDeltaTime * .1f);

        }
        if (Input.GetButtonUp("Attack"))
        {
            firing = false;
            distanceBetweenMousePoints = 0;
        }


        if (start != null && end != null && firing)
        {
            startT.position = start;
            endT.position = end;

            lr.SetPosition(0, new Vector3(start.x, start.y + 1, start.z));
            lr.SetPosition(1, new Vector3(end.x, end.y + 1, end.z));

        }

    }
    public Vector3 clampedMousePosition;

    void ClampMouseToDirectionalVector()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 clampVector = end - start;

        clampedMousePosition = VectorUtil.ClampPoint(clampVector, start, end);
        mouseBall.position = clampedMousePosition;
    }

}
