using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CollisionData {
    public Vector3 collisionPoint;
    public Vector3 collisionNormal;
    public float collisionDepth;

    public CollisionData(Vector3 collisionPoint, Vector3 collisionNormal, float collisionDepth)
    {
        this.collisionPoint = collisionPoint;
        this.collisionNormal = collisionNormal;
        this.collisionDepth = collisionDepth;
    }

}
