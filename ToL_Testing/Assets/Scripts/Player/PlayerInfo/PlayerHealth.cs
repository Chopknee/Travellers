using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{

    public TextMeshProUGUI hpUI;
    public Image hpImg, hpSubImg;

    public float health = 100;
    public float Health
    {
        get { return health; }
        set { health = value; }
    }

    public bool dotActivated;

    private void Start()
    {
        //ChangeHealth(Health, false, 15f, true, .15f);
    }

    private void FixedUpdate()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1)) // 1 is remove 15 health immediately.
        {
            ChangeHealth(Health, false, 15f, false, .15f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) // 2 is remove 15 health over time at a rate of .15f.
        {
            ChangeHealth(Health, false, 15f, true, .15f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) // 3 is remove 50 health immediately.
        {
            ChangeHealth(Health, false, 50f, false, .15f);
        }


        // hpUI.text = Health.ToString();
        hpImg.fillAmount = Health / 100;


        hpSubImg.fillAmount = Mathf.Lerp(hpSubImg.fillAmount, Health / 100, .8f * Time.deltaTime);

        if (dotActivated)
        {
            DotAffect();
        }
    }

    public float lastHealth, c01, hp, rate, next, closest;

    
    public void ChangeHealth(float lastHealth, bool heal, float hp, bool dot, float rate) // health as of calling the function, add or remove, how much to change, over time, rate
    {

        float c01;
        c01 = (heal) ? 1 : -1; // c01 is the modifier of -1 or 1

        if (dot)
        {
            this.c01 = c01;
            this.hp = hp;
            this.rate = rate;
            this.lastHealth = lastHealth;
            dotActivated = true;
            DotAffect();
        }
        else Health += c01 * hp;
    }

    private void DotAffect()
    {
       

        closest = 100;
        if (Health >= 0 && Health <= 100) // health should be equal to the health it WILL be
        {
            Health += c01 * rate;

            next = (hp * c01) + lastHealth;

            if (next < closest) closest = Mathf.Round(Mathf.Abs(Health - next));

            if (closest == 0)
            {
                Health = next;
                dotActivated = false;
            }


            


        }
        else dotActivated = false;
    }
    
}
