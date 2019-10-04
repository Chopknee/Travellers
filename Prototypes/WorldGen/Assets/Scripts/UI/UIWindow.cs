using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindow: MonoBehaviour {

    public delegate void Closed ();
    public Closed OnClosed;

    public Button closeButton;

    public void CloseGUI() {
        OnClosed?.Invoke();
        Destroy(gameObject);
    }

    public void Start () {
        closeButton.onClick.AddListener(CloseGUI);
    }

}
