using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropFood : MonoBehaviour
{
    public GameManager gameManager;
    public int price;
    void Start() {
        price = Random.Range(50, 101);
    }
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag != "Player"){
            StartCoroutine(gameManager.MinusRewardPlay(price));
        }
    }
}
