using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarUserControl : MonoBehaviour
    {
        float lastJump = 0f;
        private CarController m_Car; // the car controller we want to use
        public HealthTester EnergyTester;
        public GameManager gameManager;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            float jvf = (Input.GetAxis("GasPedal") + 1) / 2;
            float jvb = (Input.GetAxis("BreakPedal") - 1) / 2;
            float rawV = v;
            v += (jvf + jvb);
            // print(v);
            if (GameManager.playing)
            {
                if (true)//EnergyTester.curHealth > 0
                {
                    if (v != 0)
                    {
                        EnergyTester.consumeEnergy(m_Car.GetComponent<Rigidbody>().velocity.magnitude * 0.001f);
                    }
                    // Debug.Log(v);
#if !MOBILE_INPUT
                    bool jump = Input.GetAxis("Jump") > 0f;
                    if (Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.LeftShift))
                    {
                        m_Car.Move(h, v, v, 1f);
                    }
                    else
                    {
                        m_Car.Move(h, v, v, 0f);
                    }
                    if (Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.Space))
                    {
                        if (EnergyTester.curHealth >= 10)
                        {
                            Debug.Log("Jump");
                            if (Time.time - lastJump > 1f)
                            {
                                // int move;
                                // if(v == 0){
                                //     move = 0;
                                // }
                                // else if(v > 0){
                                //     move = 1;
                                // }
                                // else{
                                //     move = -1;
                                // }
                                EnergyTester.consumeEnergy(10);
                                this.gameObject.GetComponent<Rigidbody>().velocity = this.gameObject.GetComponent<Rigidbody>().velocity + new Vector3(0f, 5f, 0f);

                                lastJump = Time.time;
                            }
                        }
                        else
                        {
                            gameManager.EnergyNotEnoughTXT.SetActive(true);
                            gameManager.EnergyNotEnoughTXT.GetComponent<Animation>().Stop();
                            gameManager.EnergyNotEnoughTXT.GetComponent<Animation>().Play();
                        }
                    }

                    this.gameObject.GetComponent<Rigidbody>().angularVelocity += this.GetComponent<Transform>().TransformDirection(new Vector3(rawV * 0.01f, 0f, -1 * h * 0.01f));
#else
            m_Car.Move(h, v, v, 0f);
#endif
                }
                else
                {
                    if (Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.LeftShift))
                    {
                        m_Car.Move(h, 0, 0, 1f);
                    }
                    else
                    {
                        m_Car.Move(h, 0, 0, 0f);
                    }
                }
            }
            else if(GameManager.gameFinished)
            {
                m_Car.Move(0, 5, 0, 0f);
            }
        }
    }
}
