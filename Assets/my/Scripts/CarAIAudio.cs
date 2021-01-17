using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAIAudio : MonoBehaviour
{
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collisionInfo) {
        if (collisionInfo.collider.gameObject.tag == "AutonomousVehicle" || collisionInfo.collider.gameObject.tag == "Building" || collisionInfo.collider.gameObject.tag == "SceneObject") {
            AudioSource.PlayClipAtPoint(audioSource.clip, collisionInfo.transform.position);
        }
    }
}
