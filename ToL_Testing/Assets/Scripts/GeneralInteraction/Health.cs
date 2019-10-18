using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;
using UnityEngine.UI;
using UnityEngine.AI;
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
    public Transform playerFairy, exit;

    public GameObject deathObj, mesh;

    public GameObject particles;


    private bool dotActivated;
    public bool dead;

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


        hpSubImg.fillAmount = Mathf.Lerp(hpSubImg.fillAmount, GetHealth / maxHealth, .8f * Time.fixedDeltaTime);

        if (dotActivated)
        {
            DotAffect();
        }

        GetHealth = Mathf.Clamp(GetHealth, 0, maxHealth);


        if (GetHealth <= 0 && !dead)
        {
            if (deathObj != null && mesh != null && !GetComponent<PlayerMovement>())
            {
                Instantiate(deathObj, transform.position, Quaternion.identity);
                mesh.SetActive(false);
                mesh = null;
                Invoke("Die", .1f);
            }
            else if (GetComponent<PlayerMovement>())
            {
                dead = true;
                Respawn();
            }

        }

        if (spawn && CheckIfPlayerFairyHasArrivedAtExit())
        {
            Invoke("TimedSpawn", 2);
            spawn = false;
        }

    }

    private float lastHealth, c01, hp, rate, next, closest;
    bool spawn;

    private bool CheckIfPlayerFairyHasArrivedAtExit()
    {
        float distanceToExit = (exit.position - playerFairy.position).sqrMagnitude;
        float fdist = 14;
        playerFairy.GetComponent<NavMeshAgent>().enabled = true;

        if (distanceToExit <= fdist)
        {
            Debug.Log("kdjfsgnhkisrujnhb");
            playerFairy.GetComponent<NavMeshAgent>().enabled = false;
            playerFairy.position = Vector3.Lerp(exit.position, playerFairy.position, Time.deltaTime * 2);

            if (distanceToExit <= 4)
            {
                return true;
            }

            return false;
        }
        else
        {
            playerFairy.GetComponent<NavMeshAgent>().enabled = true;
            return false;
        }
    }




    public void Respawn()
    {
        PlayerMovement pm = GetComponent<PlayerMovement>();
        pm.playerCanInteract = false;
        PlayerDeathParticleController pdpc = transform.GetComponentInChildren<PlayerDeathParticleController>();
        pdpc.Play(pdpc.GetComponent<VisualEffect>());
        GetComponent<Animator>().SetTrigger("Die");
        GetComponent<Animator>().SetBool("Dead", true);

        PlayDeathSound();
    }

    private void PlayDeathSound()
    {
        Camera.main.GetComponent<CameraMovement>().currentTarget = playerFairy;
        GetComponent<AudioSource>().clip = GetComponent<EntityAudioClips>().GetClip("DeathSound_mixdown");
        GetComponent<AudioSource>().Play();

        GetComponent<NavMeshAgent>().isStopped = true;
        GetComponent<NavMeshAgent>().enabled = false;
        Invoke("FinishFairyEnable", 4);
    }
    private void FinishFairyEnable()
    {
        PlayerDeathParticleController pdpc = transform.GetComponentInChildren<PlayerDeathParticleController>();

        pdpc.Stop(pdpc.fxToPauseOnStart[1]);
        pdpc.Play(pdpc.fxToPauseOnStart[0]);

        mesh.SetActive(false);

        playerFairy.GetChild(0).GetComponent<Light>().enabled = true;
        playerFairy.GetComponent<NavMeshAgent>().enabled = true;
        playerFairy.gameObject.SetActive(true);
        playerFairy.GetComponent<NavMeshAgent>().SetDestination(GameObject.FindGameObjectWithTag("PortalOut").transform.position);
        spawn = true;


    }

    private void TimedSpawn()
    {
        PlayerDeathParticleController pdpc = transform.GetComponentInChildren<PlayerDeathParticleController>();
        pdpc.Stop(pdpc.fxToPauseOnStart[0]);

        transform.position = playerFairy.TransformPoint(Vector3.zero);

        mesh.SetActive(true);
        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<NavMeshAgent>().isStopped = false;
        playerFairy.GetChild(0).GetComponent<Light>().enabled = false;
        playerFairy.GetComponent<NavMeshAgent>().enabled = false;
        playerFairy.position = transform.position;
        GetComponent<Animator>().SetBool("Dead", false);
        GetComponent<PlayerMovement>().playerCanInteract = true;
        dead = false;
        ChangeHealth(true, maxHealth, false, 1f);

    }

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
