﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosionAutoDestroy : MonoBehaviour
{
    float startTime;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - startTime > 2f){
            Destroy(this.gameObject);
        }
    }
}
