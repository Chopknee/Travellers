using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionConfirmation : MonoBehaviour {

    public delegate void Result(bool res);
    public Result OnResult;

    public Button btnConfirm;
    public Text txtShortText;
    public Text txtLongText;
    public Button btnCancel;

    public RectTransform baseTransform;

    public void Start() {
        gameObject.SetActive(false);
    }

    public void Show(string longText, string shortText, Vector2 pos) {
        gameObject.SetActive(true);
        txtShortText.text = shortText;
        txtLongText.text = longText;
        btnConfirm.onClick.AddListener(Confirm);
        btnCancel.onClick.AddListener(Cancel);
        baseTransform.position = pos;
    }

    public void Update() {
        //While active, keep the dialog on screen and locked to it's relative position
        
    }

    public void Confirm() {
        OnResult?.Invoke(true);
        Close();
    }

    public void Cancel() {
        OnResult?.Invoke(false);
        Close();
    }

    public void Close() {
        gameObject.SetActive(false);
        btnCancel.onClick.RemoveAllListeners();
        btnConfirm.onClick.RemoveAllListeners();
    }
}
