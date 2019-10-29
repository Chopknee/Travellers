using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class FocusOnEnable : MonoBehaviour {
    private void OnEnable () {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
