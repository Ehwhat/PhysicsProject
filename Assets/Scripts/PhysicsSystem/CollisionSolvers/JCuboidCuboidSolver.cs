﻿using System.Collections;
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

        for (int i = 0; i < axesOfSeperation.Length; i++)
        {
            if (Vector3.SqrMagnitude(axesOfSeperation[i]) < 0.001f)// If out axis has a length of 0, some false positives can happen, soooooo lets just not do that
            {
                continue;
            }
            float depth = FindPenetrationDepth(colliderA, colliderB, axesOfSeperation[i], out shouldFlip);
            if(depth <= 0)
            {
                return false;
            }else if(depth < collision.collisionDepth)
            {
                collision.collisionDepth = depth;
                normal = shouldFlip ? axesOfSeperation[i] : axesOfSeperation[i] * -1.0f;
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

        JSegment meshSegment = JMeshCollider.GetMeshSegmentOnAxis(colliderA, sharedPlaneAxis);
        float distance = meshSegment.Length * 0.5f - collision.collisionDepth * 0.5f;
        Vector3 pointOnPlane = colliderA.transform.position + sharedPlaneAxis * distance;

        for (int i = 0; i < contacts.Count; i++)
        {
            collision.collisionPoints.Add(contacts[i] + (sharedPlaneAxis * Vector3.Dot(sharedPlaneAxis, pointOnPlane - contacts[i])));
        }

        collision.valid = true;
        collision.collisionNormal = sharedPlaneAxis;

        return true;
    }

    private float FindPenetrationDepth(JCuboidCollider colliderA, JCuboidCollider colliderB, Vector3 axis, out bool flipNormals) // 😏
    {
        JSegment segmentA = JMeshCollider.GetMeshSegmentOnAxis(colliderA, axis.normalized);
        JSegment segmentB = JMeshCollider.GetMeshSegmentOnAxis(colliderB, axis.normalized);

        if(!JSegment.IsOverlapping(segmentA, segmentB))
        {
            flipNormals = false;
            return 0;
        }

        float minSegment = Mathf.Min(segmentA.Min, segmentB.Min);
        float maxSegment = Mathf.Max(segmentA.Max, segmentB.Max);

        float length = maxSegment - minSegment;

        flipNormals = (segmentB.Min < segmentA.Min);
        return (segmentA.Length + segmentB.Length) - length;
    }

    private List<Vector3> CuboidIntersectionPoints(JCuboidCollider colliderA, JCuboidCollider colliderB)
    {
        JLineSegment[] edges = colliderA.GetEdges();
        Plane[] planes = colliderB.GetPlanes();

        List<Vector3> points = new List<Vector3>(edges.Length);
        Vector3 intersection;

        for (int i = 0; i < planes.Length; i++)
        {
            for (int j = 0; j < edges.Length; j++)
            {
                if(edges[j].IntersectWithPlane(planes[i], out intersection))
                {
                    if (colliderB.IsPointInside(intersection))
                    {
                        points.Add(intersection);
                    }
                }
            }
        }
        return points;
        
    }
}
