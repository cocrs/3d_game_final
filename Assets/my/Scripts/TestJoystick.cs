using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJoystick : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        Debug.Log(Input.GetJoystickNames().Length);
    }

    // Update is called once per frame
    void Update() {

    }
}
