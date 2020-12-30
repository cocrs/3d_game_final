using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropFood : MonoBehaviour
{
    public GameManager gameManager;
    public int price;
    private bool initializing = true;
    void Start() {
        price = Random.Range(50, 101);
        gameManager = GameObject.Find("GameMangaer").GetComponent<GameManager>();
    }
    void Update(){
        if(initializing){
            gameObject.GetComponent<Rigidbody>().velocity = gameManager.player.GetComponent<Rigidbody>().velocity;
        }
    }
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag != "Player"){
            StartCoroutine(gameManager.MinusRewardPlay(price));
        }
        else{
            initializing = false;
        }
    }
}
