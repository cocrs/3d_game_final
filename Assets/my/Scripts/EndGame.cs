using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    public GameManager gameManager;
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("in");
        if (other.gameObject.tag == "Player")
        {
            if (gameManager.playerDollars < 1000)
            {
                other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(1000, 0, 0);
            }
        }
    }
}
