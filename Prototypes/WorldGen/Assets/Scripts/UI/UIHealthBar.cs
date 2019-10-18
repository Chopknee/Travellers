using BaD.Modules.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour {


    public Image foreground;
    public Image middleground;

    float hp;
    float max;

    void Start () {
        GetComponentInParent<Health>().OnHealthChanged += HealthChanged;
        max = GetComponentInParent<Health>().MaxHealth;
    }

    void HealthChanged(float newhp) {
        hp = newhp;
        foreground.fillAmount = ( newhp / max );
    }

}
