using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct JRaycastHit {

    public bool hit;
    public JCollider hitCollider;
    public Vector3 hitNormal;
    public Vector3 hitPoint;
    public Vector3 origin;
    public float distance;
	
}
