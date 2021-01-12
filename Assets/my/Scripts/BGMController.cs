using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour {
    public AudioClip[] audioClips;
    public float musicVolume = 0.6f;
    public float time;
    AudioSource audioSource;
    int currentClip = 0;
    float fadeOut = 0f;
    // Start is called before the first frame update
    void Start() {
        audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = audioClips[currentClip];
    }

    // Update is called once per frame
    void Update() {
        time = audioSource.time;
        if (currentClip == 0) {
            if (audioSource.time > 56.0f) {
                audioSource.time = 4.4f;
            }
        }
        if (currentClip == 1) {
            if (audioSource.time > 61.0f) {
                audioSource.time = 5.0f;
            }
        }
        /*
        if (Input.GetKey(KeyCode.L)) {
            this.setClip(1);
        } else {
            this.setClip(0);
        }
        */
    }

    void FixedUpdate() {
        audioSource.volume += (this.fadeOut * this.musicVolume);
        if (audioSource.volume <= 0f) {
            audioSource.clip = audioClips[currentClip == 0 ? 1 : 0];
            currentClip = currentClip == 0 ? 1 : 0;
            if (currentClip == 0) {
                audioSource.time = 4.0f;
            } else {
                audioSource.time = 4.5f;
            }
            audioSource.Play();
            this.fadeOut *= -1;
        }
        audioSource.volume = audioSource.volume > this.musicVolume ? this.musicVolume : audioSource.volume;
        audioSource.volume = audioSource.volume < 0f ? 0f : audioSource.volume;
    }

    public void setClip(int clipNum) {
        if (currentClip != clipNum) {
            this.changeClip(clipNum);
        }
    }
    public int getCurrentClip() {
        return this.currentClip;
    }
    void changeClip(int clipNum) {
        print("Change Clip");
        if (clipNum == 0) {
            fadeOut = -0.05f;
        }
        if (clipNum == 1) {
            fadeOut = -1f;
        }
    }


}








