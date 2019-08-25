using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCSpawnInitialization : MonoBehaviour
{
    public void Initialize()
    {
        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<NPCAgression>().enabled = true;
        GetComponent<WeaponDetector>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }

}
