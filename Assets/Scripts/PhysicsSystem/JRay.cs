using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct JRay {

    public Vector3 origin;
    public Vector3 direction;

    public JRay(Vector3 origin, Vector3 direction)
    {
        this.origin = origin;
        this.direction = direction.normalized;
    }

}
