using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAttributes
{
    public float maxHealth;
    public float minHit, maxHit;
    public float critHit;
    public float attackRate;
    public bool hostile;
    public enum Difficulty
    {
        Easy, Medium, Hard
    }
    public enum DamageType
    {
        Magic, Melee, Range
    }

    public Difficulty difficulty = Difficulty.Easy;
    public DamageType dmg = DamageType.Melee;

    public NPCAttributes(float maxHealth, float critHit, float attackRate, Difficulty difficulty, DamageType dmg, bool hostile){

        this.maxHealth = maxHealth;
        this.critHit = critHit;
        this.attackRate = attackRate;
        this.difficulty = difficulty;
        this.dmg = dmg;
        this.hostile = hostile;
    }

}
