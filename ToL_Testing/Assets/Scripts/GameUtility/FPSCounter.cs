using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class FPSCounter : MonoBehaviour
{
    public float fps;
    public float avgFPS;
    public TextMeshProUGUI fpsText, avgFpsText;

    private void Awake()
    {
        StartCoroutine(CountFPS());
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
