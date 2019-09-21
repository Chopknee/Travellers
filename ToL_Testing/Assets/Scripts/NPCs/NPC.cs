using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCAttributes attributes;
    private float npcID;
    public string entityName;

    private void Awake()
    {
        attributes = new NPCAttributes(200, 10, 1, NPCAttributes.Difficulty.Easy, NPCAttributes.DamageType.Melee, true);
        npcID = Mathf.RoundToInt(Random.Range(1111, 9999));
        string name = entityName + " " + npcID.ToString();
        gameObject.name = name;
    }
}
