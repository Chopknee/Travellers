using UnityEngine;

namespace BaD.Modules.Combat {

    public class PlayerHealth: Health {

        private new void FixedUpdate () {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) // 1 is remove 15 health immediately.
            {
                ChangeHealth(false, 100f, true, 2f);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) // 2 is remove 15 health over time at a rate of .15f.
            {
                ChangeHealth(false, 600f, true, 4f);
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3)) // 3 is remove 50 health immediately.
            {
                ChangeHealth(false, 50f, false, 1f);
            }

            base.FixedUpdate();
        }
    }
}
