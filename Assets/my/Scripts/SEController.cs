﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEController : MonoBehaviour {
    public AudioClip[] audioClips;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start() {
        audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {

    }

    void playClip(int clipNum) {
        this.audioSource.PlayOneShot(audioClips[clipNum]);
    }
}
