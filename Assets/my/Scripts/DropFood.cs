using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropFood : MonoBehaviour
{
    public GameManager gameManager;
    void Start() {

    }
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag != "Player"){
            gameManager.minusEnergy();
        }
    }
}
