//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Experimental.Rendering.HDPipeline;
//public class WaypointAnimations : MonoBehaviour
//{
//    private float emission = 1.5f;
//    float expandSpeed = 2f;
//    float shapeRadius = 1f;
//    float intensity;
//    Color originColor, originColor2;
//    float defaultIntensity, defaultIntensity2, defaultRadius, defaultEmission;
//    Color defaultColor;
//    Light l, l2;
//    public bool decrease, dead, expanded;

//    private void Awake()
//    {
//        l = GetComponent<Light>();
//        l2 = transform.root.GetComponent<Light>();
//        decrease = false;


//        defaultIntensity = l.intensity;
//        defaultRadius = l.GetComponent<HDAdditionalLightData>().shapeRadius;
//        defaultEmission = l.spotAngle;
//        defaultColor = l.color;
//        defaultIntensity2 = transform.root.GetComponent<Light>().intensity;

//    }

//    private void Update()
//    {
//        l.spotAngle = emission;
//        l.GetComponent<HDAdditionalLightData>().shapeRadius = shapeRadius;
//        l2.GetComponent<HDAdditionalLightData>().shapeRadius = shapeRadius;
//        if (shapeRadius > 0)
//        {
//            shapeRadius -= .03f;
//        }
//        if (!expanded && !decrease && l.spotAngle < 50)
//        {

//            emission += expandSpeed;

//        }
//        else if (l.spotAngle >= 50)
//        {
//            expanded = true;
//        }


//        if (dead)
//        {
//            l.intensity -= 2f;
//            l.GetComponent<HDAdditionalLightData>().shapeRadius += .01f;
//            l.spotAngle += .1f;
//            l.color = originColor;
//            l2.intensity -= .5f;
//            l2.GetComponent<HDAdditionalLightData>().shapeRadius += .01f;
//            originColor.a -= .2f;
//        }
//        else
//        {


//            if (l.intensity < defaultIntensity) l.intensity += 2f;
//            if(l2.intensity < defaultIntensity2) l2.intensity = Mathf.Clamp(l2.intensity += 2f, 0, defaultIntensity2);
//            l.GetComponent<HDAdditionalLightData>().shapeRadius = Mathf.Clamp(l.GetComponent<HDAdditionalLightData>().shapeRadius -= .01f, 0, defaultRadius);
//            //l.spotAngle = Mathf.Clamp(l.spotAngle -= .1f, 0, defaultEmission);

//            //l.color = originColor;
//            //originColor.a += .2f;
//        }


//    }

//    public void Die()
//    {
//        originColor = l.color;
//        originColor2 = transform.root.GetComponent<Light>().color;
//        intensity = l.intensity;
//        transform.root.tag = "Untagged";
//        Debug.Log("Dead");
//        dead = true;
//        StartCoroutine(LifeSpan());
//        StartCoroutine(Dead());
//    }
//    IEnumerator LifeSpan()
//    {
//        yield return new WaitForSeconds(4);
//        dead = true;
//        StartCoroutine(Dead());
//    }
//    IEnumerator Dead()
//    {

//        yield return new WaitForSeconds(2);

//        Destroy(transform.root.gameObject);

//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;
public class WaypointAnimations : MonoBehaviour
{
    private float emission = 1.5f;
    float expandSpeed = 2f;
    float shapeRadius = 1f;
    float intensity;
    Color originColor, originColor2;
    float defaultIntensity, defaultIntensity2, defaultRadius, defaultEmission;
    Color defaultColor;
    float distToPlayer;
    Transform player;
    Light l, l2;
    public bool decrease, dead, expanded;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        l = GetComponent<Light>();
        l2 = transform.root.GetComponent<Light>();
        decrease = false;


        defaultIntensity = l.intensity;
        defaultRadius = l.GetComponent<HDAdditionalLightData>().shapeRadius;
        defaultEmission = l.spotAngle;
        defaultColor = l.color;

        originColor = defaultColor;
        originColor2 = l2.color;
        defaultIntensity2 = transform.root.GetComponent<Light>().intensity;

    }

    private void Update()
    {
        if (player == null) { Destroy(gameObject); return; }

        distToPlayer = Vector3.Distance(transform.position, player.position);
        l.spotAngle = emission;
        l.GetComponent<HDAdditionalLightData>().shapeRadius = shapeRadius;
        l2.GetComponent<HDAdditionalLightData>().shapeRadius = shapeRadius;
        if (shapeRadius > 0)
        {
            shapeRadius -= .03f;
        }
        if (!expanded && !decrease && l.spotAngle < 50)
        {
            emission += expandSpeed;
        }
        else if (l.spotAngle >= 50)
        {
            expanded = true;
        }

        if (distToPlayer <= 3)
        {
            dead = true;
        }

        if (dead)
        {
            l.intensity -= 2f;
            l.GetComponent<HDAdditionalLightData>().shapeRadius += .01f;
            l.spotAngle += .1f;
            l.color = originColor;
            l2.intensity -= .5f;
            l2.GetComponent<HDAdditionalLightData>().shapeRadius += .01f;
            originColor.a -= .2f;
        }
        else
        {
            if (l.intensity < defaultIntensity) l.intensity += 2f;
            if (l2.intensity < defaultIntensity2) l2.intensity = Mathf.Clamp(l2.intensity += 2f, 0, defaultIntensity2);
            l.GetComponent<HDAdditionalLightData>().shapeRadius = Mathf.Clamp(l.GetComponent<HDAdditionalLightData>().shapeRadius -= .01f, 0, defaultRadius);
        }
    }

    public void Die()
    {
        originColor = l.color;
        originColor2 = transform.root.GetComponent<Light>().color;
        intensity = l.intensity;

        StartCoroutine("LifeSpan");
    }
    IEnumerator LifeSpan()
    {
        dead = true;
        yield return new WaitForSeconds(4);

        Destroy(transform.root.gameObject);
    }
}

