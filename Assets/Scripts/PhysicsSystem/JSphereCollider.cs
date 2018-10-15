using System.Collections;
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

    private void Start()
    {
        
    }

    public override Bounds GetBounds()
    {
        return bounds;
    }

    public override bool TestCollisionWith(JSphereCollider collider, out CollisionData collision)
    {
        return CollisionSolvers.SphereVsSphereCollision(new Sphere(transform.position, _radius), new Sphere(collider.transform.position, collider._radius), out collision); 
    }

    public override bool TestCollisionWith(JPlaneCollider collider, out CollisionData collision)
    {
        return CollisionSolvers.SphereVsPlaneCollision(new Sphere(transform.position, _radius), new Plane(collider.Normal,collider.transform.position), out collision);
    }

    public override bool TestCollisionWith(JCuboidCollider collider, out CollisionData collision)
    {
        collision = new CollisionData();
        return false;
    }

    public override Bounds GenerateBounds()
    {
        bounds = new Bounds(Vector3.zero, _radius * 2 * Vector3.one);
        return bounds;
    }
}
