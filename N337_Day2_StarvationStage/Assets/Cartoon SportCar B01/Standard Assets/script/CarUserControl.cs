using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Photon.Pun;
using Photon.Realtime;

namespace UnityStandardAssets.Vehicles.Car

{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviourPun
    {
        private CarController m_Car; // the car controller we want to use

        private void Awake()
        {
            if (!GetComponent<PhotonView>().IsMine) return;
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
            if (!GetComponent<PhotonView>().IsMine) return;
            
                // pass the input to the car!
                float h = CrossPlatformInputManager.GetAxis("Horizontal");
                float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
                float handbrake = CrossPlatformInputManager.GetAxis("Jump");
                m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
            
        }
    }
}
