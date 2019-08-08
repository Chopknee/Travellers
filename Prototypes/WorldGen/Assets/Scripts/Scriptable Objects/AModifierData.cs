using System;
using UnityEngine;

namespace BaD.Modules.Terrain.Modifiers {
    public abstract class AModifierData: ScriptableObject {
        public abstract void Execute (Map map);
    }
}