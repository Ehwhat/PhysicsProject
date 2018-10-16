using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyForceOnKeypress : MonoBehaviour {

    public Vector3 force;
    public JRigidbody body;

    private void Start()
    {
        body = GetComponent<JRigidbody>(); 
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.D))
        {
            body.ApplyImpulse(force);
        }
	}
}
