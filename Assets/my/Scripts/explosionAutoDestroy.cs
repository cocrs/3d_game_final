using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosionAutoDestroy : MonoBehaviour
{
    public float duration;
    float startTime;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - startTime > duration){
            Destroy(this.gameObject);
        }
    }
}
