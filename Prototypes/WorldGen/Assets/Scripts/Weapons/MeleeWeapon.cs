using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public enum WeaponType
    {
        Dagger, Sword, GreatSword, Mace, Hammer
    }

    public WeaponType weapon = WeaponType.Dagger;

    public float attackRate = 1;

    public float baseDamage = 100f;
}
