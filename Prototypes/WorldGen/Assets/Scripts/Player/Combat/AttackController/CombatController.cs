using UnityEngine;
using UnityEngine.InputSystem;

namespace BaD.Modules.Combat {

    public class CombatController: MonoBehaviour {
        Animator anim;

        public GameObject northWeapon;
        public GameObject eastWeapon;
        public GameObject westWeapon;

        AttackStyle weaponScript;



        private void Start () {

            //Set up to listen for changes to the currently selected items


            if (northWeapon.GetComponent<AttackStyle>() == null) {
                Debug.Log("Cannot use current weapon gameobject. It does not have an attack style or derivative script.");
                return;
            }
            SetWeapon(northWeapon.GetComponent<AttackStyle>());

        }

        void Update () {

        }

        public void DoAttack(InputAction.CallbackContext context) {
            //
            if (northWeapon != null) {
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
            northWeapon = weapon.gameObject;
            weaponScript = weapon;
            weaponScript.wielder = gameObject;
        }
    }
}