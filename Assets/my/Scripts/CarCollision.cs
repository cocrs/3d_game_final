using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CarCollision : MonoBehaviour
{
    float lastCollideTime;
    public GameObject EnergyTester;
    public CinemachineVirtualCamera cinemachine;
    void Start()
    {
        lastCollideTime = Time.time;
    }
    void OnCollisionEnter(Collision other)
    {
        if (Time.time - lastCollideTime > 1f)
        {
            lastCollideTime = Time.time;

            // if(other.transform.parent.tag == "Building"){
            //     GameObject.Find("GameManager").GetComponent<GameManager>().adjustScore(-100);
            // }
            if (other.gameObject.tag == "AutonomousVehicle")
            {
                EnergyTester.GetComponent<HealthTester>().consumeEnergy(2);
                StartCoroutine(collideEffect());
            }           

        }
    }
    IEnumerator collideEffect()
    {
        cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 2f;
        yield return new WaitForSeconds(0.1f);
        cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
    }
}
