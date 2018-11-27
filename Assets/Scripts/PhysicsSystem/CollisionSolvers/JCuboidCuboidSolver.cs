using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JCuboidCuboidSolver : JSATSolver<JCuboidCollider, JCuboidCollider>
{
    protected override bool CheckCollision(JCuboidCollider colliderA, JCuboidCollider colliderB, out JCollision collision)
    {
        collision = new JCollision();
        Vector3[] axesOfSeperation = GetAxesOfSeperation(colliderA, colliderB);
        Vector3 normal = Vector3.zero;
        bool shouldFlip = false;
        collision.collisionDepth = float.PositiveInfinity;
        collision.colliderA = colliderA;
        collision.colliderB = colliderB;

        Vector3[] colliderAVerts = colliderA.GetVertices();
        Vector3[] colliderBVerts = colliderB.GetVertices();

        for (int i = 0; i < axesOfSeperation.Length; i++)
        {
            if (Vector3.SqrMagnitude(axesOfSeperation[i]) < 0.001f)// If out axis has a length of 0, some false positives can happen, soooooo lets just not do that
            {
                continue;
            }
            float depth = FindPenetrationDepth(colliderAVerts, colliderBVerts, axesOfSeperation[i], out shouldFlip);
            if(depth <= 0)
            {
                return false;
            }else if(depth < collision.collisionDepth)
            {
                collision.collisionDepth = depth;
                normal = shouldFlip ? axesOfSeperation[i] * -1.0f :  axesOfSeperation[i];
            }
        }
        if(normal == Vector3.zero)
        {
            return false;
        }
        Vector3 sharedPlaneAxis = normal.normalized;

        List<Vector3> contacts = new List<Vector3>();
        contacts.AddRange(CuboidIntersectionPoints(colliderB, colliderA));
        contacts.AddRange(CuboidIntersectionPoints(colliderA, colliderB));

        

        JSegment meshSegment = JMeshCollider.GetMeshSegmentOnAxis(colliderAVerts, sharedPlaneAxis);
        float distance = (meshSegment.Length * 0.5f) - (collision.collisionDepth * 0.5f);
        Vector3 pointOnPlane = colliderA.transform.position + sharedPlaneAxis * distance;

        collision.collisionNormal = sharedPlaneAxis;

        collision.valid = contacts.Count > 0;

        for (int i = 0; i < contacts.Count; i++)
        {
            Vector3 contact = contacts[i];
            contact = contact + (sharedPlaneAxis * Vector3.Dot(sharedPlaneAxis, pointOnPlane - contact));

            bool unique = true;
            for (int j = 0; j < collision.collisionPoints.Count; j++)
            {
                if (Vector3.Distance(contact,collision.collisionPoints[j]) < 0.2f)
                    unique = false;
            }
            if(unique)
                collision.collisionPoints.Add(contact);
        }



        return collision.valid;
    }

    private float FindPenetrationDepth(Vector3[] colliderAVertices, Vector3[] colliderBVertices, Vector3 axis, out bool flipNormals) // 😏
    {
        JSegment segmentA = JMeshCollider.GetMeshSegmentOnAxis(colliderAVertices, axis.normalized);
        JSegment segmentB = JMeshCollider.GetMeshSegmentOnAxis(colliderBVertices, axis.normalized);

        flipNormals = (segmentB.Min < segmentA.Min);

        if (!JSegment.IsOverlapping(segmentA, segmentB))
        {
            return 0;
        }

        float minSegment = Mathf.Min(segmentA.Min, segmentB.Min);
        float maxSegment = Mathf.Max(segmentA.Max, segmentB.Max);

        float length = maxSegment - minSegment;

        
        return (segmentA.Length + segmentB.Length) - length;
    }

    private List<Vector3> CuboidIntersectionPoints(JCuboidCollider colliderA, JCuboidCollider colliderB)
    {
        JLineSegment[] edges = colliderA.GetEdges();
        Plane[] planes = colliderB.GetPlanes();

        List<Vector3> points = new List<Vector3>(edges.Length);
        Vector3 intersection;
        int pointsFound = 0;

        for (int i = 0; i < planes.Length; i++)
        {
            for (int j = 0; j < edges.Length; j++)
            {
                if(edges[j].IntersectWithPlane(planes[i], out intersection))
                {
                    if (colliderB.IsPointInside(intersection))
                    {
                        points.Add(intersection);
                        pointsFound++;
                        if(pointsFound > 4)
                        {
                            return points;
                        }
                    }

                }
            }
        }
        return points;
        
    }
}
