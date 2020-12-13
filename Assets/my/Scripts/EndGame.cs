using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    public GameObject successTXT;
    public GameManager gameManager;
    public GameObject[] btns;
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("in");
        if (other.gameObject.tag == "Player")
        {
            if (gameManager.playerDollars < 1000)
            {
                other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(1000, 0, 0);
            }
            else{
                this.GetComponent<BoxCollider>().isTrigger = true;
                successTXT.SetActive(true);
                gameManager.PlayerControll(false);
                btns[0].SetActive(false);
                btns[1].SetActive(false);
            }
        }
    }
}
