using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStyle: MonoBehaviour {
    public enum AttackType {
        Ranged, Melee, Magic
    }
    public AttackType attackStyle;

    public bool IsAttacking { get; protected set; }
    public GameObject wielder;

    public virtual void DoAttack () {
        //This is just the base class, it should really never be used...
    }

}
