
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

namespace BaD.Modules.Combat {

    /*
     * the health is already set up to kill the object IF both the mesh and death obj have been set. 
     * If so, then it disables "mesh" (which is the reference to the original character MESH (not root)... 
     * then it spawns the death obj in its place (this can be anything -- I'm using bones with rigidbodies 
     * for the skeleton, but could be a ragdoll). Leave these empty if you don't want to have that functionality yet. 
     * Add this script to any possible NPC or player
    */
    //
    public class Health: MonoBehaviour, IPunObservable {

        public GameObject deathObj, mesh;

        [SerializeField]
#pragma warning disable 0649//Disables the warning about unused vars
        private float InitialMaxHealth = 200;//Variable just for the editor.

        public float health { get; private set; }

        public float MaxHealth { get; private set; }

        public bool currentlyDamaging { get; private set; }

        public delegate void HealthChanged ( float value );
        public HealthChanged OnHealthChanged;


        private void Start () {
            MaxHealth = InitialMaxHealth;
            health = InitialMaxHealth;

            lastHP = health;

        }

        protected void FixedUpdate () {
            if (currentlyDamaging) {
                DotAffect();
            }

            health = Mathf.Clamp(health, 0, MaxHealth);//Keeping health between 0 and max health

            if (health <= 0 && deathObj != null && mesh != null) {
                Instantiate(deathObj, transform.position, Quaternion.identity);
                mesh.SetActive(false);
                mesh = null;
                Invoke("Die", .1f);
            }
        }

        float lastHP = 0;
        protected void Update () {
            if (lastHP != health) {
                OnHealthChanged?.Invoke(health);
                lastHP = health;
            }
        }

        private float lastHealth, c01, hp, rate, next, closest;


        public void Die () {
            NetInstanceManager.CurrentManager.DestroyObject(gameObject);
        }

        // health as of calling the function, add or remove, how much to change, over time, rate
        public void ChangeHealth ( bool heal, float hp, bool damageOverTime, float rate ) {

            float c01;
            c01 = ( heal ) ? 1 : -1; // c01 is the modifier of -1 or 1

            if (damageOverTime) {
                this.c01 = c01;
                this.hp = hp;
                this.rate = rate;
                lastHealth = health;
                currentlyDamaging = true;
                DotAffect();
            } else {
                health += c01 * hp;
                Debug.Log(gameObject.name + " took " + hp.ToString() + " damage.");
            }
        }

        private void DotAffect () {
            closest = MaxHealth;
            // health should be equal to the health it WILL be
            if (health >= 0 && health <= MaxHealth) {
                health += c01 * rate;

                next = ( hp * c01 ) + lastHealth;

                if (next < closest) closest = Mathf.Round(Mathf.Abs(health - next));

                if (closest == 0) {
                    health = next;
                    currentlyDamaging = false;
                }
            } else {
                currentlyDamaging = false;
            }
        }

        public void OnPhotonSerializeView ( PhotonStream stream, PhotonMessageInfo info ) {
            //if (!PhotonNetwork.IsMasterClient) {
            //    stream.SendNext(health);
            //} else {
            //    health = (float) stream.ReceiveNext();
            //}
        }
    }
}