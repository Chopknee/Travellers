using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UpdateNavMesh : MonoBehaviour
{
    private NavMeshSurface nav;
    // Start is called before the first frame update
    void Start()
    {
        nav = this.GetComponent<NavMeshSurface>();
        Invoke("BuildNavMesh", .5f);
    }

    void BuildNavMesh()
    {
        nav.BuildNavMesh();
    }
    
}
