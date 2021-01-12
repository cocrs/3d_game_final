using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;

public class carBGMController : MonoBehaviour {
    public GameObject bgmControllerObject;
    BGMController bgmController;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start() {
        bgmController = bgmControllerObject.GetComponent<BGMController>();
        audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        Collider[] nearCar = UnityEngine.Physics.OverlapSphere(this.transform.position, 30f);
        bool changeClip = false;
        foreach (Collider c in nearCar) {
            if (c.gameObject.GetComponent<CarAI>() != null) {
                if (c.gameObject.GetComponent<CarAI>().crushIntoPlayer == true) {
                    changeClip = true;
                }
            }
        }
        bgmController.setClip(changeClip ? 1 : 0);
    }
    void OnCollisionEnter(Collision collisionInfo) {
        if (collisionInfo.collider.gameObject.tag == "AutonomousVehicle") {
            AudioSource.PlayClipAtPoint(audioSource.clip, collisionInfo.transform.position);
        }
    }
}
