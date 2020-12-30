using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropFood : MonoBehaviour
{
    public GameManager gameManager;
    public int price;
    private bool initializing = true;
    private bool dropped = false;
    void Awake() {
        price = Random.Range(50, 101);
        gameManager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
    }
    void Update(){
        if(initializing){ 
            Vector3 tmp = gameManager.player.GetComponent<Rigidbody>().velocity;
            tmp[1] = gameObject.GetComponent<Rigidbody>().velocity.y;
            gameObject.GetComponent<Rigidbody>().velocity = tmp;
        }
    }
    private void OnCollisionEnter(Collision other) {
        if(!dropped && other.gameObject.tag != "Player" && other.gameObject.tag != "Food"){
            print("drop");
            dropped = true;
            StartCoroutine(gameManager.MinusRewardPlay(price));
            Destroy(gameObject, 5);
        }
        else{
            initializing = false;
        }
    }
}
