using BaD.Modules.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    public static MainControls mainControls;
    // Start is called before the first frame update
    void Awake()
    {
        mainControls = new MainControls();
        mainControls.Enable();
    }
}
