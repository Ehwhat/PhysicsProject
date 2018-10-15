using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JCollision {

    public bool valid;

    public Vector3 collisionPoint;
    public Vector3 collisionNormal;
    public float collisionDepth;

    public JCollider colliderA;

    public JCollider colliderB;

    public JCollision()
    {
        valid = false;
    }

    public JCollision(Vector3 point, Vector3 normal, float depth, JCollider colliderA, JCollider colliderB)
    {
        valid = true;
        collisionPoint = point;
        collisionNormal = normal;
        collisionDepth = depth;
        this.colliderA = colliderA;
        this.colliderB = colliderB;
    }

    public JCollision(CollisionData data, JCollider colliderA, JCollider colliderB)
    {
        valid = true;
        collisionPoint = data.collisionPoint;
        collisionNormal = data.collisionNormal;
        collisionDepth = data.collisionDepth;
        this.colliderA = colliderA;
        this.colliderB = colliderB;
    }

    public override bool Equals(object obj)
    {
        if(obj == null)
        {
            return false;
        }

        JCollision collision = (JCollision)obj;
        if(collision != null)
        {
            bool equal =
                (colliderA == collision.colliderA && colliderB == collision.colliderB) ||
                (colliderA == collision.colliderB && colliderB == collision.colliderA);
            return equal;
        }
        return false;
    }
    public static bool operator==(JCollision c1, JCollision c2)
    {
        if (object.ReferenceEquals(c1, null))
        {
            return object.ReferenceEquals(c2, null);
        }

        return c1.Equals(c2);
    }
    public static bool operator !=(JCollision c1, JCollision c2)
    {
        return !(c1 == c2);
    }
}
