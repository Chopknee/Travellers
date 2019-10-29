using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonFixer: MonoBehaviour {


    public delegate void Clicked ( ButtonFixer caller );
    public Clicked OnClicked;

    Button b;

    // Start is called before the first frame update
    void Start () {
        b = GetComponent<Button>();
        b.onClick.AddListener(WasClicked);
    }

    private void WasClicked() {
        OnClicked?.Invoke(this);
    }
}
