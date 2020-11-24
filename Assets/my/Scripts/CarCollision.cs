using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollision : MonoBehaviour
{
    float lastCollideTime;
    void Start(){
        lastCollideTime = Time.time;
    }
    private void OnCollisionEnter(Collision other) {
        if(Time.time - lastCollideTime > 1f){
            lastCollideTime = Time.time;
            if(other.transform.parent != null){
                if(other.transform.parent.tag == "Building"){
                    GameObject.Find("GameManager").GetComponent<GameManager>().adjustScore(-100);
                }
                if(other.gameObject.tag == "AutonomousVehicle"){
                GameObject.Find("GameManager").GetComponent<GameManager>().adjustScore(-200);
                }
            }
            
        }
    }
}
