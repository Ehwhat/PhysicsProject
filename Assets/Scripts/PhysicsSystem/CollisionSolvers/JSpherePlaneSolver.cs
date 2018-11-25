using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSpherePlaneSolver : JCollisionSolver<JSphereCollider, JPlaneCollider>
{
    protected override bool CheckCollision(JSphereCollider colliderA, JPlaneCollider colliderB, out JCollision collision)
    {

        Vector3 closestPointA = colliderB.GetClosestPoint(colliderA.transform.position);
        Vector3 closestPointB = colliderA.GetClosestPoint(closestPointA, true);

        Debug.DrawLine(closestPointA, closestPointB);

        if (colliderB.IsPointInside(closestPointB))
        {
            float collisionDepth = Vector3.Distance(closestPointA, closestPointB);
            Vector3 collisionNormal = (closestPointA - closestPointB).normalized;
            List<Vector3> contacts = new List<Vector3>() { closestPointA + (closestPointB * 0.5f) };

            collision = new JCollision(contacts, collisionNormal, collisionDepth, colliderA, colliderB);
            return true;
        }
        collision = null;
        return false;
    }
}
