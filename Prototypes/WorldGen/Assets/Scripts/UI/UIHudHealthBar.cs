using BaD.Modules;
using BaD.Modules.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHudHealthBar : MonoBehaviour {


    public Image foreground;
    public Image middleground;
    public TextMeshProUGUI healthText;


    Health hps;
    GameObject lastPlyaerInst;

    void Start () {
        GetComponent<CanvasGroup>().alpha = 0;
    }

    private void Update () {
        if (DungeonManager.CurrentInstance != null) {
            
            if (DungeonManager.CurrentInstance.LocalDungeonPlayerInstance != null) {
                if (lastPlyaerInst != DungeonManager.CurrentInstance.LocalDungeonPlayerInstance) {
                    GetComponent<CanvasGroup>().alpha = 1;
                    hps = DungeonManager.CurrentInstance.LocalDungeonPlayerInstance.GetComponent<Health>();
                    hps.OnHealthChanged = HealthChanged;
                }
            }
        } else {
            GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    void HealthChanged(float newhp, GameObject damager) {
        foreground.fillAmount = ( newhp / hps.MaxHealth );
        healthText.text = newhp + "/" + hps.MaxHealth;// + ((damager != null)? " " + damager.name : "");
    }

}
