using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCAttributes attributes;
    public float health;

    private void Awake()
    {
        attributes = new NPCAttributes(100, 10, 1, NPCAttributes.Difficulty.Easy, NPCAttributes.DamageType.Melee, true);
    }
}
