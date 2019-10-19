using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

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

        }

        public void DoAttack(InputAction.CallbackContext context) {
            //
            if (currentWeapon != null) {
                if (!weaponScript.IsAttacking) {
                    weaponScript.DoAttack();
                }
            }
        }

        public void OnEnable() {
            MainControl.Instance.Controls.Player.Attack.performed += DoAttack;
        }

        public void OnDisable() {
            MainControl.Instance.Controls.Player.Attack.performed -= DoAttack;
        }

        public void SetWeapon ( AttackStyle weapon ) {
            currentWeapon = weapon.gameObject;
            weaponScript = weapon;
            weaponScript.wielder = gameObject;
        }
    }
}