﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionShapes;

public class JSphereCollider : JCollider {

    public float Radius
    {
        get { return _radius; }
        set {
            _radius = value;
        }
    }

    private Bounds bounds;

    [SerializeField]
    private float _radius;

    [SerializeField]
    private Transform _testTransform;

    private void Start()
    {
        
    }

    public override Bounds GetBounds()
    {
        return bounds;
    }

    public override Vector3 GetClosestPoint(Vector3 fromPoint, bool clampToEdge = false)
    {
        if(Vector3.Distance(transform.position, fromPoint) > _radius || clampToEdge)
        {
            return transform.position + (fromPoint - transform.position).normalized * _radius;
        }
        return fromPoint;
    }

    public override bool IsPointInside(Vector3 point)
    {
        return Vector3.Distance(transform.position, point) <= _radius;
    }

    public override Bounds GenerateBounds()
    {
        bounds = new Bounds(Vector3.zero, _radius * 2 * Vector3.one);
        return bounds;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _radius);
        Vector3 closestPoint = GetClosestPoint(_testTransform.position);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(closestPoint, 0.1f);
    }
}
