using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSphereSphereSolver : JCollisionSolver<JSphereCollider, JSphereCollider>
{
    

    protected override bool CheckCollision(JSphereCollider colliderA, JSphereCollider colliderB, out JCollision collision)
    {
        Vector3 closestPointA = colliderA.GetClosestPoint(colliderB.transform.position);
        Vector3 closestPointB = colliderB.GetClosestPoint(colliderA.transform.position);

        Debug.DrawLine(closestPointA, closestPointB, Color.red);

        if((closestPointB - closestPointA).sqrMagnitude >= 0)
        {
            float collisionDepth = Vector3.Distance(closestPointA, closestPointB);
            Vector3 collisionNormal = (closestPointB - closestPointA).normalized;
            List<Vector3> contacts = new List<Vector3>() { closestPointA + (closestPointB * 0.5f) };

            collision = new JCollision(contacts, collisionNormal, collisionDepth, colliderA, colliderB);
            return true;
        }
        collision = null;
        return false;
        
    }
}
