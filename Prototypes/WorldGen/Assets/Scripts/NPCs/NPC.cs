using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCAttributes attributes;
    public float radiusForAttacking;
    public float attackRadSquared { get; private set; }

    private void Awake()
    {
        attributes = new NPCAttributes(200, 10, 1, NPCAttributes.Difficulty.Easy, NPCAttributes.DamageType.Melee, true);
        attackRadSquared = radiusForAttacking * radiusForAttacking;
    }

    private void OnDrawGizmos () {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radiusForAttacking);
    }
}
