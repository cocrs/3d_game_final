using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car {
    [RequireComponent(typeof(CarController))]
    public class CarUserControl : MonoBehaviour {
        float lastJump = 0f;
        private CarController m_Car; // the car controller we want to use


        private void Awake() {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate() {
            // pass the input to the car!
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            float jvf = (Input.GetAxis("GasPedal") + 1) / 2;
            float jvb = (Input.GetAxis("BreakPedal") - 1) / 2;
            float rawV = v;
            v += (jvf + jvb);
            // Debug.Log(v);
#if !MOBILE_INPUT
            bool jump = Input.GetAxis("Jump") > 0f;
            m_Car.Move(h, v, v, 0f);
            if (Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.Space)) {
                Debug.Log("Jump");

                if (Time.time - lastJump > 1f) {
                    this.gameObject.GetComponent<Rigidbody>().velocity = this.gameObject.GetComponent<Rigidbody>().velocity + new Vector3(0f, 5f, 0f);
                    lastJump = Time.time;
                }
            }
            this.gameObject.GetComponent<Rigidbody>().angularVelocity += this.GetComponent<Transform>().TransformDirection(new Vector3(rawV * 0.01f, 0f, -1 * h * 0.01f));
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
