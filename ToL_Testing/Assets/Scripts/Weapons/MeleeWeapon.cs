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

    public float attackRate = .2f;
}
