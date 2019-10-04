using BaD.Modules.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon: AttackStyle {

    public float baseDamage = 100f;
    public float knockbackPower = 5;
    public float attackRate = .2f;



    public Vector3 damagePosition = Vector3.zero;
    public float damageRadius = 1;
    private float damageRadiusSquared;
    public float attackDamageDelay = 0.2f;
    public string[] DamageableTags = { "Enemy" };//Set to whatever object(s) should be damaged

    public Vector3 DamageCenter {
        get {
            //return ( transform.rotation * damagePosition ) + transform.position;
            return ( wielder.transform.rotation * damagePosition ) + wielder.transform.position;
        }
    }

    private void Awake () {
        damageRadiusSquared = damageRadius * damageRadius;
    }

    public override void DoAttack () {
        if (wielder.GetComponent<Animator>() != null) {
            wielder.GetComponent<Animator>().SetTrigger("Attack");
        }
        //Drop the damage orb somewhere??
        IsAttacking = true;
        Invoke("DoDamage", attackDamageDelay);
        Invoke("SetCanAttack", attackRate);
    }

    void SetCanAttack () {
        IsAttacking = false;
    }

    void DoDamage () {
        Collider[] hitColliders = Physics.OverlapSphere(DamageCenter, damageRadius);
        foreach (Collider c in hitColliders) {
            bool isAttackable = false;
            //Looking to see if the current object can be attacked.
            foreach (string checkTag in DamageableTags) {
                if (checkTag == c.tag) {
                    isAttackable = true;
                    break;
                }
            }
            //If no matching tag was found, move to the next object.
            if (!isAttackable) { continue; }

            //Since we found an object, check if it can be damaged. (if it has a health script)
            if (c.gameObject.GetComponent<Health>() != null) {
                Vector3 dir = -( c.transform.position - DamageCenter );

                //                c.GetComponent<Rigidbody>().AddForce(dir.normalized * knockbackPower);
                c.GetComponent<Health>().ChangeHealth(false, baseDamage, false, 1);
            }
        }
    }

    public void OnDrawGizmosSelected () {
        if (wielder != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(DamageCenter, damageRadius);
        }
    }
}