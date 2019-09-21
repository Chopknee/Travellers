using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : AttackStyle
{
    public enum WeaponType
    {
        Bow
    }

    public WeaponType weapon = WeaponType.Bow;

    

    public float baseDamage = 100f;
}
