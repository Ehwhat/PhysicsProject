﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JMeshCollider : JCollider {

    public abstract Vector3[] GetVertices();

    public static JSegment GetMeshSegmentOnAxis(Vector3[] vertices, Vector3 axis)
    {
        float projection = Vector3.Dot(axis, vertices[0]);
        JSegment segment = new JSegment(projection, projection);

        for (int i = 1; i < vertices.Length; i++)
        {
            projection = Vector3.Dot(axis, vertices[i]);
            if (projection < segment.Min)
            {
                segment.Min = projection;
            }
            if (projection > segment.Max)
            {
                segment.Max = projection;
            }
        }
        return segment;
    }

}
