using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class FPSCounter : MonoBehaviour
{
    public float fps, avgFPS, animSpeed;
    public TextMeshProUGUI fpsText, avgFpsText, animSpeedTxt;

    private void Awake()
    {
        StartCoroutine(CountFPS());
    }

    private void FixedUpdate()
    {
        animSpeed = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>().GetFloat("Runspeed");
        animSpeedTxt.text = animSpeed.ToString("0.00");
    }

    int c = 0;
    IEnumerator CountFPS()
    {
        while (true)
        {
            c++;

            
            
            fps = 1 / Time.deltaTime;
            avgFPS += fps;
            
            avgFpsText.text = (avgFPS / c).ToString("0.00");
            fpsText.text = fps.ToString("0.00");
            yield return new WaitForSeconds(1);
        }
    }
}
