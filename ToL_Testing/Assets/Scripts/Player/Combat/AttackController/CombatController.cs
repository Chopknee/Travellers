using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatController : MonoBehaviour
{
    Animator anim;
    public TextMeshProUGUI t;

    public GameObject currentWeapon;
    Collider hitbox;

    float lastHit;



    private void Start()
    {
        anim = GetComponent<Animator>();
        hitbox = currentWeapon.transform.Find("Hitbox").GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Attack") && Time.time - lastHit > hitbox.transform.parent.GetComponent<MeleeWeapon>().attackRate)
        {
            t.text = "Attacking";

            if (hitbox != null)
            {
                Invoke("BeginCollision", .7f);
            }
            anim.SetTrigger("Attack");
        }
    }

    void BeginCollision()
    {
        hitbox.enabled = true;
        Invoke("EndCollision", .5f);
    }
    void EndCollision()
    {
        t.text = "";
        if (hitbox != null)
            hitbox.enabled = false;
    }
}
