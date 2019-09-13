using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class CombatController : MonoBehaviour, IPunObservable
{
    Animator anim;
    public TextMeshProUGUI t;

    public GameObject currentWeapon;
    Collider hitbox;

    float lastHit;

    PhotonView view;
    bool attacking = false;
    public bool canAttack = false;

    /*
     * make movement and attack the same button, and only attack if clicking on an enemy.
     * 
     */

    private void Start() {
        anim = GetComponent<Animator>();
        hitbox = currentWeapon.transform.Find("Hitbox").GetComponent<Collider>();
        view = GetComponent<PhotonView>();
        if (!view.IsMine) {
            canAttack = true;
        }
    }

    // Update is called once per frame
    void Update() {
        if (view.IsMine && canAttack) {
            if (Input.GetButtonDown("Attack") && Time.time - lastHit > hitbox.transform.parent.GetComponent<MeleeWeapon>().attackRate) {
                if (t != null) {
                    t.text = "Attacking";
                }
                anim.SetTrigger("Attack");
                attacking = true;
            }
        }

        if (NetInstanceManager.CurrentManager.isInstanceMaster) {
            if (attacking) {
                Invoke("BeginCollision", 0.7f);
            }
        }
    }

    //This should only be run on the master of the instance
    void BeginCollision() {
        if (hitbox != null) {
            hitbox.enabled = true;
        }
        Invoke("EndCollision", .5f);
    }
    
    void EndCollision() {
        if (t != null) {
            t.text = "";
        }
        if (hitbox != null)
            hitbox.enabled = false;

        attacking = false;
    }

    public void OnPhotonSerializeView ( PhotonStream stream, PhotonMessageInfo info ) {
        if (view != null) {
            if (view.IsMine) {
                //I'm sending the messages now bitch!
                stream.SendNext(attacking);
            } else if (!view.IsMine && !attacking) {
                attacking = (bool) stream.ReceiveNext();
            }
        }
    }
}
