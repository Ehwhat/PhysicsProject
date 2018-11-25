using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JSATSolver<T1,T2> : JCollisionSolver<T1, T2> where T1:JMeshCollider where T2: JMeshCollider
{

    protected bool IsOverlapping(JMeshCollider colliderA, JMeshCollider colliderB)
    {
        Vector3[] axesOfSeperation = GetAxesOfSeperation(colliderA, colliderB);
        for (int i = 0; i < 15; i++)
        {
            if(!IsOverlapingOnAxis(colliderA,colliderB, axesOfSeperation[i]))
            {
                return false;
            }
        }
        return true;
    }

    protected Vector3[] GetAxesOfSeperation(JMeshCollider colliderA, JMeshCollider colliderB)
    {
        return new Vector3[]
        {
            colliderA.transform.right,
            colliderA.transform.up,
            colliderA.transform.forward,

            colliderB.transform.right,
            colliderB.transform.up,
            colliderB.transform.forward,

            Vector3.Cross(colliderA.transform.right, colliderB.transform.right),
            Vector3.Cross(colliderA.transform.up, colliderB.transform.right),
            Vector3.Cross(colliderA.transform.forward, colliderB.transform.right),

            Vector3.Cross(colliderA.transform.right, colliderB.transform.up),
            Vector3.Cross(colliderA.transform.up, colliderB.transform.up),
            Vector3.Cross(colliderA.transform.forward, colliderB.transform.up),

            Vector3.Cross(colliderA.transform.right, colliderB.transform.forward),
            Vector3.Cross(colliderA.transform.up, colliderB.transform.forward),
            Vector3.Cross(colliderA.transform.forward, colliderB.transform.forward)

        };
    }

    protected bool IsOverlapingOnAxis(JMeshCollider colliderA, JMeshCollider colliderB, Vector3 axis)
    {
        JSegment segmentA = JMeshCollider.GetMeshSegmentOnAxis(colliderA, axis);
        JSegment segmentB = JMeshCollider.GetMeshSegmentOnAxis(colliderB, axis);
        return JSegment.IsOverlapping(segmentA, segmentB);
    }



}
