using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct JContact {

    public Vector3 position;
    public Vector3 normal;
    public float depth;

    public Vector3 accumulatedNormalImpulse;
    public Vector3 accumulatedFrictionImpulse;

    public float normalMass;
    public float frictionMass;

    public JContact(Vector3 position, Vector3 normal, float depth)
    {
        accumulatedNormalImpulse = Vector3.zero;
        accumulatedFrictionImpulse = Vector3.zero;

        normalMass = 0;
        frictionMass = 0;

        this.position = position;
        this.normal = normal;
        this.depth = depth;
    }
	
}
