using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonController : MonoBehaviour {

    private Animator animator;
    private float timer;
    private float changeTime;
    private bool changeDirection = false;
    private bool died = false;
    private bool endRoutine = true;
    private float lastRotateTime;
    public Transform player;

    // Start is called before the first frame update
    void Start() {
        animator = gameObject.GetComponent<Animator>();
        timer = 0;
        changeTime = Random.Range(0f, 10f);
        lastRotateTime = Time.time;
        player = GameObject.Find("Car").transform;
    }

    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;
        if (!died) {
            if (changeDirection && endRoutine) {
                endRoutine = false;
                if (Random.Range(0, 2) == 0) {
                    StartCoroutine(RotateMe(new Vector3(0, 180, 0), 5));
                } else {
                    StartCoroutine(RotateMe(new Vector3(0, -180, 0), 5));
                }
                changeDirection = false;
                animator.SetInteger("state", 1);
            } else if (timer > changeTime && endRoutine) {
                // print("change");
                int state = Random.Range(0, 7);
                if (state > 4) {
                    animator.SetInteger("state", 0);
                } else if (state > 0) {
                    animator.SetInteger("state", 1);
                } else {
                    animator.SetInteger("state", 2);
                }
                timer = 0;
                changeTime = Random.Range(0, 10);
            } else if ((animator.GetInteger("state") == 1 || animator.GetInteger("state") == 2) && endRoutine && Time.time - lastRotateTime > 30) {
                endRoutine = false;
                Vector3 rotate = new Vector3(0, Random.Range(-45f, 45f), 0);
                StartCoroutine(RotateMe(rotate, 5));
                lastRotateTime = Time.time;
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (!died) {
            if (other.gameObject.tag == "Player" || other.gameObject.tag == "AutonomousVehicle") {
                print(other.gameObject.GetComponent<Rigidbody>().velocity.magnitude);
                if (other.gameObject.GetComponent<Rigidbody>().velocity.magnitude - other.gameObject.GetComponent<Rigidbody>().velocity.y > 8) {
                    animator.SetInteger("state", 3);
                    died = true;
                    Destroy(gameObject, 10);
                }
            } else if ((animator.GetInteger("state") == 1 || animator.GetInteger("state") == 2) && (other.gameObject.tag == "Pavement" || other.gameObject.tag == "Road")) {
                //print("jump");
                // animator.SetInteger("state", 0);
                gameObject.GetComponent<Rigidbody>().velocity += new Vector3(0, 3f, 0) + gameObject.transform.forward;
            } else if (other.gameObject.tag != "Untagged") {
                animator.SetInteger("state", 0);
                changeDirection = true;
            }

        }
    }
    private void OnCollisionStay(Collision other) {
        if (!died) {
            if (other.gameObject.tag == "Player" || other.gameObject.tag == "AutonomousVehicle") {
                Transform tmp = player;
                tmp.position = new Vector3(tmp.position.x, gameObject.transform.position.y, tmp.position.z);
                transform.LookAt(tmp);
                animator.SetInteger("state", 4);

            }
        }
    }
    IEnumerator RotateMe(Vector3 byAngles, float inTime) {
        // print("start");
        var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);
        for (var t = 0f; t < 1; t += Time.deltaTime / inTime) {
            transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
            yield return null;
        }
        endRoutine = true;
        // print("end");
    }

}
