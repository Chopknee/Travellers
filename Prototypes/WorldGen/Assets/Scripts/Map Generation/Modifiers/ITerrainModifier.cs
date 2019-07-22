using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITerrainModifier
{
    int GetPriority();

    void Modify (Map map);
}
