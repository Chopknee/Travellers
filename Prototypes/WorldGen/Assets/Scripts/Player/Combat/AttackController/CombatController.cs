using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BaD.Modules.Combat {

    public class CombatController: MonoBehaviour {
        Animator anim;

        public GameObject currentWeapon;
        AttackStyle weaponScript;



        private void Start () {
            if (currentWeapon.GetComponent<AttackStyle>() == null) {
                Debug.Log("Cannot use current weapon gameobject. It does not have an attack style or derivative script.");
                return;
            }
            SetWeapon(currentWeapon.GetComponent<AttackStyle>());
        }

        void Update () {
            if (currentWeapon != null) {
                if (UnityEngine.Input.GetButtonDown("Attack")) {
                    if (!weaponScript.IsAttacking) {
                        weaponScript.DoAttack();
                        //Debug.Log("Attacking!!");
                    } else {
                        //Debug.Log("Can't attack, already attacking.");
                    }
                }
            }
        }

        public void SetWeapon ( AttackStyle weapon ) {
            currentWeapon = weapon.gameObject;
            weaponScript = weapon;
            weaponScript.wielder = gameObject;
        }
    }
}