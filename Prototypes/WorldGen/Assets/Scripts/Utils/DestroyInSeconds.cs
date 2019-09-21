using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInSeconds : MonoBehaviour {

    public float time = 1;

    public void StartTimeout() {
        Invoke("Die", time);
    }

    void Die() {
        Destroy(gameObject);
    }
}
