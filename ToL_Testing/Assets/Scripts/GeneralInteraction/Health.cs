using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



/*
 * the health is already set up to kill the object IF both the mesh and death obj have been set. 
 * If so, then it disables "mesh" (which is the reference to the original character MESH (not root)... 
 * then it spawns the death obj in its place (this can be anything -- I'm using bones with rigidbodies 
 * for the skeleton, but could be a ragdoll). Leave these empty if you don't want to have that functionality yet. 
 * Add this script to any possible NPC or player
*/

public class Health : MonoBehaviour
{
    public float health = 200, maxHealth = 200;
    public TextMeshProUGUI hpUI;
    public Image hpImg, hpSubImg;

    public GameObject deathObj, mesh;

    private bool dotActivated;

    public float GetHealth
    {
        get { return health; }
        set { health = value; }
    }

    public float GetMaxHealth
    {
        get { return GetHealth; }
        set { GetHealth = value; }
    }

    private void Start()
    {
        GetHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        Health h = GetComponent<Health>();

        if (transform.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) // 1 is remove 15 health immediately.
            {
                h.ChangeHealth(false, 100f, true, 2f);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2)) // 2 is remove 15 health over time at a rate of .15f.
            {
                h.ChangeHealth(false, 600f, true, 4f);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)) // 3 is remove 50 health immediately.
            {
                h.ChangeHealth(false, 50f, false, 1f);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha4)) // 1 is remove 15 health immediately.
            {
                h.ChangeHealth(false, 100f, true, 2f);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5)) // 2 is remove 15 health over time at a rate of .15f.
            {
                h.ChangeHealth(false, 600f, true, 4f);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6)) // 3 is remove 50 health immediately.
            {
                h.ChangeHealth(false, 50f, false, 1f);
            }
        }


        hpImg.fillAmount = health / maxHealth;
        hpUI.text = health.ToString() + " / " + maxHealth.ToString();

        hpSubImg.fillAmount = Mathf.Lerp(hpSubImg.fillAmount, health / maxHealth, .8f * Time.fixedDeltaTime);

        if (dotActivated)
        {
            DotAffect();
        }

        GetHealth = Mathf.Clamp(GetHealth, 0, maxHealth);

        if (GetHealth <= 0 && deathObj != null && mesh != null)
        {
            Instantiate(deathObj, transform.position, Quaternion.identity);
            mesh.SetActive(false);
            mesh = null;
            Invoke("Die", .1f);
        }

    }

    private float lastHealth, c01, hp, rate, next, closest;


    public void Die()
    {
        Destroy(gameObject);
    }

    public void ChangeHealth(bool heal, float hp, bool dot, float rate) // health as of calling the function, add or remove, how much to change, over time, rate
    {

        float c01;
        c01 = (heal) ? 1 : -1; // c01 is the modifier of -1 or 1

        if (dot)
        {
            this.c01 = c01;
            this.hp = hp;
            this.rate = rate;
            lastHealth = GetHealth;
            dotActivated = true;
            DotAffect();
        }
        else
        {
            GetHealth += c01 * hp;
            Debug.Log(gameObject.name + " took " + hp.ToString() + " damage.");
        }
    }

    private void DotAffect()
    {
        closest = maxHealth;
        if (GetHealth >= 0 && GetHealth <= maxHealth) // health should be equal to the health it WILL be
        {
            GetHealth += c01 * rate;

            next = (hp * c01) + lastHealth;

            if (next < closest) closest = Mathf.Round(Mathf.Abs(GetHealth - next));

            if (closest == 0)
            {
                GetHealth = next;
                dotActivated = false;
            }
        }
        else dotActivated = false;
    }


}
