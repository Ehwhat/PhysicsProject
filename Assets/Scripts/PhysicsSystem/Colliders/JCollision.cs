using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JCollision {

    public bool valid = false;

    public Vector3 collisionNormal = Vector3.zero;
    public float collisionDepth = 0;
    public List<Vector3> collisionPoints = new List<Vector3>();

    public JCollider colliderA;

    public JCollider colliderB;

    public JCollision()
    {
        valid = false;
    }

    public JCollision(List<Vector3> points, Vector3 normal, float depth, JCollider colliderA, JCollider colliderB)
    {
        valid = true;
        collisionPoints = points;
        collisionNormal = normal;
        collisionDepth = depth;
        this.colliderA = colliderA;
        this.colliderB = colliderB;
    }

    public JCollision(JCollider colliderA, JCollider colliderB)
    {
        valid = false;
        this.colliderA = colliderA;
        this.colliderB = colliderB;
    }

    public Vector3 AverageCollisionPoint()
    {
        if(collisionPoints.Count < 1)
        {
            return Vector3.zero;
        }
        Vector3 result = collisionPoints[0];
        for (int i = 1; i < collisionPoints.Count; i++)
        {
            result += collisionPoints[i];
        }
        return result /= collisionPoints.Count;
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
