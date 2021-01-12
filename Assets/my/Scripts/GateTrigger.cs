using UnityEngine;
using Fungus;

public class GateTrigger : MonoBehaviour
{
    public GameManager gameManager;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (gameManager.playerDollars >= 0)
            {
                Flowchart.BroadcastFungusMessage("playerWithEnoughMoney");
            }
            else
            {
                Flowchart.BroadcastFungusMessage("playerWithNoEnoughMoney");
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && animator.GetInteger("state") == 1)
        {
            animator.SetInteger("state", 2);
        }
    }
}

