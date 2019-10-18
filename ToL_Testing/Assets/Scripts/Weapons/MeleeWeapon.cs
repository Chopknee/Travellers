using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : AttackStyle
{
    public enum WeaponType
    {
        Dagger, Sword, GreatSword, Mace, Hammer
    }

    public WeaponType weapon = WeaponType.Dagger;

    public float baseDamage = 100f;
    public float knockbackPower = 5;
    public float attackRate = 1.5f;
    public float animationOffset = .5f;



    public Vector3 damagePosition = Vector3.zero;
    public float damageRadius = 1;
    private float damageRadiusSquared;
    
    public string[] DamageableTags = { "Enemy" };//Set to whatever object(s) should be damaged
    private Collider[] hitColliders;

    public Vector3 DamageCenter
    {
        get
        {
            return (wielder.transform.rotation * damagePosition) + wielder.transform.position;
        }
    }

    private void Awake()
    {
        damageRadiusSquared = damageRadius * damageRadius;
    }
    
    public override void DoAttack()
    {
        if (wielder.GetComponent<PlayerMovement>() != null && !wielder.GetComponent<PlayerMovement>().playerCanInteract) return;
        if (IsAttacking) return;
        IsAttacking = true;


        Animator a = wielder.GetComponent<Animator>();
        if (a != null)
        {
            a.SetTrigger("Attack");
            
            //attackRate = a.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        }
        //Drop the damage orb somewhere??
        
        Invoke("DoDamage", a.GetCurrentAnimatorClipInfo(0)[0].clip.length / 2 + animationOffset);
        Invoke("SetCanAttack", attackRate);
    }

    void SetCanAttack()
    {
        IsAttacking = false;
    }
   

    void DoDamage()
    {
        hitColliders = Physics.OverlapSphere(DamageCenter, damageRadius);
        foreach (Collider c in hitColliders)
        {
            bool isAttackable = false;
            //Looking to see if the current object can be attacked.
            foreach (string checkTag in DamageableTags)
            {
                if (c.CompareTag(checkTag))
                {
                    isAttackable = true;
                    break;
                }
            }
            //If no matching tag was found, move to the next object.
            if (!isAttackable) { continue; }

            //Since we found an object, check if it can be damaged. (if it has a health script)
            if (c.gameObject.GetComponent<Health>() != null)
            {
                Vector3 dir = -(c.transform.position - new Vector3(transform.position.x, c.transform.position.y, transform.position.z));

                c.GetComponent<Rigidbody>().AddForce(-dir.normalized * knockbackPower, ForceMode.Impulse);
                c.GetComponent<Health>().ChangeHealth(false, baseDamage, false, 1);
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (wielder != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(DamageCenter, damageRadius);
        }
    }
}
