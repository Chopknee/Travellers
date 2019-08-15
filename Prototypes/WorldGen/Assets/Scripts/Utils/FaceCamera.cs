using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BaD.Chopknee.Utilities {
    public class FaceCamera: MonoBehaviour {
        void Update () {
            transform.LookAt(Camera.main.transform);
        }
    }
}
