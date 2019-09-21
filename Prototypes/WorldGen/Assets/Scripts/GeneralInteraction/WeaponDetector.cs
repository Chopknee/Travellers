using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDetector : MonoBehaviour
{
    public Health h;
    
    private void OnTriggerEnter(Collider other)
    {
        if (NetInstanceManager.CurrentManager.isInstanceMaster) {
            if (other.CompareTag("SwingingWeapon")) {
                MeleeWeapon w = other.transform.parent.GetComponent<MeleeWeapon>();
                Debug.Log("Weapon: " + w.weapon + " hit " + gameObject.name + " with " + w.baseDamage);
                h.ChangeHealth(false, w.baseDamage, false, 1);
                other.enabled = false;
            }
        }
    }
}
