using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    private void OnTriggerStay(Collider other) {
        print("in");
        if(other.tag == "Player"){
            
            other.GetComponent<Rigidbody>().AddForce(new Vector3(100, 0 ,0));
        }
    }
}
