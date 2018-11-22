using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JCuboidCuboidSolver : JCollisionSolver<JCuboidCollider, JCuboidCollider>
{
    protected override bool CheckCollision(JCuboidCollider colliderA, JCuboidCollider colliderB, out JCollision collision)
    {
        Vector3 closestPointA = colliderA.GetClosestPoint(colliderB.transform.position, true);
        Vector3 closestPointB = colliderB.GetClosestPoint(closestPointA, true);
        closestPointA = colliderA.GetClosestPoint(closestPointB, true);
        closestPointB = colliderB.GetClosestPoint(closestPointA, true);

        Debug.DrawLine(closestPointA, closestPointB, Color.red);

        if (colliderB.IsPointInside(closestPointA))
        {
            Debug.Log("Test");
            float collisionDepth = Vector3.Distance(closestPointA, closestPointB);
            Vector3 collisionPoint = closestPointA + (closestPointB * 0.5f);
            Vector3 collisionNormal = (closestPointB - closestPointA).normalized;

            collision = new JCollision(collisionPoint, collisionNormal, collisionDepth, colliderA, colliderB);
            return true;
        }
        collision = null;
        return false;
     }
}
