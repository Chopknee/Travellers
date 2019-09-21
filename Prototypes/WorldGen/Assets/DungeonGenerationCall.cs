using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerationCall : MonoBehaviour
{
    DungeonManager dm;
    private void Awake()
    {
        dm = GetComponent<DungeonManager>();
        dm.Generate();
    }
}
