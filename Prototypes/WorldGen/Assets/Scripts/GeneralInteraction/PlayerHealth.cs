using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.VFX;

namespace BaD.Modules.Combat {

    public class PlayerHealth: Health {

        public GameObject playerFairy;

        public override void Start () {
            nma = GetComponent<NavMeshAgent>();
            oldSpeed = nma.speed;
            oldRad = nma.radius;
            base.Start();
        }

        private new void FixedUpdate () {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) // 1 is remove 15 health immediately.
            {
                ChangeHealth(false, 100f, true, 2f);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) // 2 is remove 15 health over time at a rate of .15f.
            {
                ChangeHealth(false, 600f, true, 4f);
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3)) // 3 is remove 50 health immediately.
            {
                ChangeHealth(false, 50f, false, 1f);
            }

            base.FixedUpdate();
        }

        bool flying = false;
        Transform exit;

        public override void Die() {
            if (GetComponent<PhotonView>().IsMine) {
                MainControl.Instance.SetPlayerControl(false);
            }
            //Respawning the player back in the overworld
            PlayerDeathParticleController pdpc = transform.GetComponentInChildren<PlayerDeathParticleController>();
            pdpc.Play(pdpc.GetComponent<VisualEffect>());
            GetComponent<Animator>().SetTrigger("Die");
            GetComponent<Animator>().SetBool("Dead", true);

            //Play the death sound
            GetComponent<AudioSource>().clip = GetComponent<EntityAudioClips>().GetClip("DeathSound_mixdown");
            GetComponent<AudioSource>().Play();
            Invoke("FinishFairyEnable", 4);
        }

        private void FinishFairyEnable () {
            PlayerDeathParticleController pdpc = transform.GetComponentInChildren<PlayerDeathParticleController>();

            pdpc.Stop(pdpc.fxToPauseOnStart[1]);
            pdpc.Play(pdpc.fxToPauseOnStart[0]);

            mesh.SetActive(false);

            playerFairy.transform.GetChild(0).GetComponent<Light>().enabled = true;
            playerFairy.gameObject.SetActive(true);
            nma.speed = 10;
            nma.radius = 0.18f;

            exit = DungeonManager.CurrentInstance.exitPortal;

            if (NetInstanceManager.CurrentManager.isInstanceMaster) {
                nma.SetDestination(exit.position);
            }

            flying = true;
        }

        private void Respawn() {
            PlayerDeathParticleController pdpc = transform.GetComponentInChildren<PlayerDeathParticleController>();
            pdpc.Stop(pdpc.fxToPauseOnStart[0]);
            mesh.SetActive(true);
            playerFairy.transform.GetChild(0).GetComponent<Light>().enabled = true;
            playerFairy.gameObject.SetActive(false);
            GetComponent<Animator>().SetBool("Dead", false);
            if (GetComponent<PhotonView>().IsMine) {
                MainControl.Instance.SetPlayerControl(true);
            }
            ChangeHealth(true, MaxHealth, false, 1f);
        }

        NavMeshAgent nma;
        float oldSpeed;
        float oldRad;

        public override void Update () {
            //Waiting until the exit has been reached
            if (flying) {
                Vector3 pos = new Vector3(exit.position.x, transform.position.y, exit.position.z);
                //Check if the player is close enough to the exit to respawn
                float dist = ( pos - transform.position ).sqrMagnitude;
                if (dist < nma.stoppingDistance * nma.stoppingDistance) {
                    Invoke("Respawn", 2);
                    flying = false;
                    nma.isStopped = true;
                    nma.speed = oldSpeed;
                    nma.radius = oldRad;
                }

            }
            base.Update();
        }
    }
}