using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatController : MonoBehaviour
{
    Animator anim;
    public TextMeshProUGUI t;

    public GameObject currentWeapon;
    public Transform currentTarget;
    AttackStyle style;
    Collider hitbox;
    LayerMask enemyLayer;
    public float reach;
    public List<GameObject> nearbyEnemies;

    bool attacking;

    float lastHit;
    float nextHit;
   
    /*
     * make movement and attack the same button, and only attack if clicking on an enemy.
     * 
     */
    Transform s = null;
    Vector3 sRest;

    private void Start()
    {
        anim = GetComponent<Animator>();
        style = GetComponent<AttackStyle>();

    }
    
    // Update is called once per frame
    void FixedUpdate()
    {


        if (Input.GetButtonDown("Attack") && Time.time > lastHit && !attacking)
        {
            if (GetComponent<PlayerMovement>().clickedObject.CompareTag("Enemy"))
            {
                Vector3 directionOfTarget = GetComponent<PlayerMovement>().clickedObject.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionOfTarget - transform.position);
                transform.rotation = targetRotation;
            }

            anim.SetTrigger("Attack");
            
            GameObject o = new GameObject("Dmg");
            o.transform.position = transform.position + transform.forward * 1;
            Collider[] hitColliders = Physics.OverlapSphere(o.transform.position, reach);

            foreach (Collider c in hitColliders)
            {
                if (c.gameObject.GetComponent<Health>() != null)
                {
                    if (!nearbyEnemies.Contains(c.gameObject) && c.CompareTag("Enemy"))
                        nearbyEnemies.Add(c.gameObject);

                }
                else
                {
                    //Debug.Log("null" + c.name);
                }
            }
            Destroy(o);
            

            if (GetComponent<AttackStyle>().attackStyle == AttackStyle.AttackType.Melee)
            {
                waitingForHit = true;
                nextHit = Time.time + currentWeapon.GetComponent<MeleeWeapon>().attackRate;


                attacking = true;
                WaitForHit();

            }
        }
        if (waitingForHit)
        {
            WaitForHit();
        }
        
    }
    bool waitingForHit;

    void WaitForHit()
    {
        if (Time.time >= nextHit)
        {
            foreach (GameObject g in nearbyEnemies)
            {
                MeleeWeapon wp = currentWeapon.GetComponent<MeleeWeapon>();
                Vector3 dir = -(g.transform.position - transform.position);

                g.GetComponent<Rigidbody>().AddForce(-dir.normalized * wp.knockbackPower, ForceMode.Impulse); 
                g.GetComponent<Health>().ChangeHealth(false, wp.baseDamage, false, 1);
                lastHit = Time.time + anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            }
            Invoke("ClearNearbyList", .5f);
            waitingForHit = false;
            nextHit = Time.time * 2;
        }
    }
    

    void ClearNearbyList()
    {
        foreach(GameObject g in nearbyEnemies) 
        {
            if(g != null)
                g.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        nearbyEnemies.Clear();
        attacking = false;
    }









    void LetGo()
    {
        s.parent = currentWeapon.transform.Find("Main.Bone");
        s.localPosition = sRest;
    }
}
