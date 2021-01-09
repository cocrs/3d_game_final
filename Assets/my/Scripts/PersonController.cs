using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonController : MonoBehaviour
{

    private Animator animator;
    private float timer;
    private float changeTime;
    private bool changeDirection = false;
    private bool died = false;
    private bool endRoutine = true;
    private float lastRotateTime;
    private float lastCollideTime;
    private float detectDis;
    private float detectRadius;
    public bool jump = false;
    public bool bottomNotDetected = true;


    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        timer = 0;
        changeTime = Random.Range(0f, 10f);
        lastRotateTime = Time.time;
        lastCollideTime = Time.time;
        detectDis = 0.4f;
        detectRadius = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (!died)
        {
            if (timer > changeTime && endRoutine)
            {
                // print("change");
                int state = Random.Range(0, 8);
                if (state > 5)
                {
                    animator.SetInteger("state", 0);
                }
                else if (state > 1)
                {
                    animator.SetInteger("state", 1);
                }
                else if (state == 1)
                {
                    animator.SetInteger("state", 2);
                }
                else if (endRoutine){
                    endRoutine = false;
                    StartCoroutine(RotateMe(new Vector3(0, 90, 0), 2));
                }
                timer = 0;
                changeTime = Random.Range(0, 10);
            }
            RaycastHit bottomHit;
            RaycastHit topHit;
            Ray bottomRay = new Ray(transform.position, transform.forward);
            Ray topRay = new Ray(transform.position + new Vector3(0, 1.5f, 0), transform.forward);
            if (Physics.Raycast(transform.position, transform.forward, detectDis, 1 << 0) || Physics.SphereCast(transform.position, detectRadius, transform.forward, out bottomHit, detectDis, 1 << 0))
            {
                bottomNotDetected = false;
                if (!Physics.Raycast(transform.position, transform.forward, detectDis, 1 << 0) && !Physics.SphereCast(transform.position + new Vector3(0, 1.5f, 0), detectRadius, transform.forward, out topHit, detectDis, 1 << 0))
                {
                    jump = true;
                    if ((animator.GetInteger("state") == 1 || animator.GetInteger("state") == 2))
                    {
                        // print("jump");
                        // animator.SetInteger("state", 0);
                        gameObject.GetComponent<Rigidbody>().velocity += new Vector3(0, 0.5f, 0) + gameObject.transform.forward;
                    }
                }
                else
                {
                    jump = false;
                    animator.SetInteger("state", 0);
                    // changeDirection = true;F
                    if (endRoutine)
                    {
                        endRoutine = false;
                        StartCoroutine(RotateMe(new Vector3(0, 90, 0), 2));
                    }
                }
            }
            else{
                jump = false;
                bottomNotDetected = true;
            }

            // else if ((animator.GetInteger("state") == 1 || animator.GetInteger("state") == 2) && endRoutine && Time.time - lastRotateTime > 30) {
            //     endRoutine = false;
            //     Vector3 rotate = new Vector3(0, Random.Range(-45f, 45f), 0);
            //     StartCoroutine(RotateMe(rotate, 5));
            //     lastRotateTime = Time.time;
            // }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * detectDis;
        Gizmos.DrawRay(transform.position, direction);
        Gizmos.DrawWireSphere(transform.position + direction, detectRadius);
        Gizmos.DrawRay(transform.position + new Vector3(0, 1.5f, 0), direction);
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 1.5f, 0) + direction, detectRadius);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!died)
        {
            if (other.gameObject.tag == "Player" || other.gameObject.tag == "AutonomousVehicle")
            {
                // print(other.gameObject.GetComponent<Rigidbody>().velocity.magnitude);
                if (other.gameObject.GetComponent<Rigidbody>().velocity.magnitude - other.gameObject.GetComponent<Rigidbody>().velocity.y > 10)
                {
                    StartCoroutine(GameObject.FindWithTag("Manager").GetComponent<GameManager>().MinusMoneyPlay(1000));
                    animator.SetInteger("state", 3);
                    died = true;
                    Destroy(gameObject, 10);
                }
            }


        }
    }
    private void OnCollisionStay(Collision other)
    {
        if (!died)
        {
            if (other.gameObject.tag == "Player" || other.gameObject.tag == "AutonomousVehicle")
            {
                if(other.gameObject.tag == "Player" && Time.time - lastCollideTime > 5){
                    StartCoroutine(GameObject.FindWithTag("Manager").GetComponent<GameManager>().MinusMoneyPlay(100));
                    lastCollideTime = Time.time;
                }
                // Transform tmp = other.gameObject.transform;
                // tmp.position = new Vector3(tmp.position.x, gameObject.transform.position.y, tmp.position.z);
                // transform.LookAt(tmp);
                animator.SetInteger("state", 4);
            }
        }
    }
    IEnumerator RotateMe(Vector3 byAngles, float inTime)
    {
        // print("start");
        var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);
        for (var t = 0f; t < 1; t += Time.deltaTime / inTime)
        {
            transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
            yield return null;
        }
        endRoutine = true;
        // print("end");
    }

}
