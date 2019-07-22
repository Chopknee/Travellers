using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Map map;
    private Camera mainCamera;
    //Point and click movement
    //Has a maximum movement ability

    void Start() {
        Invoke("start", 1);
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void start() {
        map = Control.Instance.Map;
        Tile occupyingTile = map.tileManager.GetTile(map.RealWorldToTerrainCoord(transform.position));
        if (occupyingTile != null) {
            transform.position = occupyingTile.position + new Vector3(0, 1, 0);
        }

    }

    Vector3 lastClickPoint = Vector3.zero;

    float resolution = 0.1f;

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            
            //Mouse Click

            //Figure out where on the terrain the mouse was clicked
            for (float i = 0; i < mainCamera.farClipPlane; i+=resolution) {
                Vector2 mousePos = Input.mousePosition;
                Vector3 mouseWorldPoint = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, i));
                Tile t = map.tileManager.GetTile(map.RealWorldToTerrainCoord(mouseWorldPoint));
                if (t != null) {
                    lastClickPoint = t.position;
                    break;
                }
                //lastClickPoint = mouseWorldPoint;
                //Debug.Log("Mouse Down!! " + mouseWorldPoint);
            }
        }
    }

    Color c = new Color(0, 0, 255);
    void OnDrawGizmos() {
        Gizmos.color = c;
        Gizmos.DrawCube(lastClickPoint, Vector3.one * 10);
    }
}
