using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalMainLightAnimation : MonoBehaviour
{
    float pulse = 0;
    int dir = -1;

    public float pulseSpeed = 10f;



    // Update is called once per frame
    void Update()
    {

        GetComponent<Light>().intensity = pulse;

        

        pulse += pulseSpeed * dir;

        

        if (pulse >= 1000) dir = -1; else if(pulse <= 100) dir = 1;
    }
}
