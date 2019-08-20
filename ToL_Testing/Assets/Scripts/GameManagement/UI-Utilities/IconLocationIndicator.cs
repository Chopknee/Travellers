using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconLocationIndicator : MonoBehaviour
{
    public Transform target;
    public Image icon;
    float size = 100;
    Vector3 directionOfTarget;
    public Canvas c;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {


        /*
         * //angle of center of screen to mouse position (this will end up being center of screen to target position)
         * float angle = Mathf.Atan2(screenpos.y, screenpos.x);
         * angle -= 90 * Mathf.Deg2Rad; // he doesn't explain this shit at all
         * 
         * float cos = Mathf.Cos(angle);
         * float sin = Mathf.Sin(angle);
         * 
         * // easy enough so far.
         * 
         * screenpos = screenCenter + new Vector3(sin * 150, cos * 150, 0); // I believe he just messed with the numbers until it met the edge of the screen?
         * 
         * //y = mx+b -- we don't need the +b, so it's realy just mx.
         * 
         * //m (the slope) is cosine over sine.
         * 
         * float m = cos / sin;
         * 
         * Vector3 screenBounds = screenCenter * 0.9f; // unsure of how this is equal to the screen bounds but I imagine he's playing on a very fixed resolution
         * 
         * 
         * // top and bottom checks
         * if(cos > 0{
         *  screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0); // x / m apparently...
         * } else {
         * screenPos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0); // the exact same as before just negative -_-
         * }
         * 
         * 
         * 
         * 
         * 
         * 
         */














        Vector3 worldPos = Camera.main.WorldToScreenPoint(target.position);
        RectTransform r = c.GetComponent<RectTransform>();
        icon.rectTransform.localPosition = worldPos;

        float px = icon.rectTransform.localPosition.x;
        float py = icon.rectTransform.localPosition.y;
        
        Vector2 minimum, maximum;
        
        maximum = (r.sizeDelta - icon.rectTransform.sizeDelta) * .5f;
        minimum = (r.sizeDelta - icon.rectTransform.sizeDelta) * -.5f;
        //maximum = new Vector2(r.sizeDelta.x - icon.rectTransform.sizeDelta.x, r.sizeDelta.y);
        Debug.Log("Minimum, " + minimum + " Maximum, " + maximum);

        px = Mathf.Clamp(px, minimum.x, maximum.x);
        py = Mathf.Clamp(py, minimum.y, maximum.y);

        icon.rectTransform.localPosition = new Vector3(px, py);
        
    }
    
    private void OnDrawGizmos()
    {
        Ray ray;
        ray = new Ray(GameObject.FindGameObjectWithTag("Player").transform.position, target.position);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(ray);
    }
}
