using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionShapes;

public class JPlaneCollider : JCollider
{

    public Vector3 Normal = new Vector3(0, 1, 0);
    private Bounds bounds;
    public Plane plane;

    public override Bounds GetBounds()
    {
        return bounds;
    }

    public override Bounds GenerateBounds()
    {
        float rightMod = Mathf.Max(1000,100000 * (1 - Vector3.Dot(Normal, Vector3.right)));
        float upMod = Mathf.Max(1000, 100000 * (1 - Vector3.Dot(Normal, Vector3.up)));
        float forwardMod = Mathf.Max(1000, 100000 * (1 - Vector3.Dot(Normal, Vector3.forward)));
        bounds = new Bounds(transform.position, new Vector3(rightMod, upMod, forwardMod));
        return bounds;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        plane = new Plane(Normal, transform.position);
        transform.up = Normal;
    }

    public override bool TestCollisionWith(JSphereCollider collider, out CollisionData collision)
    {
        return CollisionSolvers.SphereVsPlaneCollision(new Sphere(collider.transform.position, collider.Radius), new Plane(Normal, transform.position), out collision);
    }

    public override bool TestCollisionWith(JPlaneCollider collider, out CollisionData collision)
    {
        collision = new CollisionData();
        return false;
    }

    public override bool TestCollisionWith(JCuboidCollider collider, out CollisionData collision)
    {
        return CollisionSolvers.CubiodVsPlaneCollision(new Cuboid(collider.transform.position, collider.Dimensions / 2), new Plane(Normal, transform.position), out collision);
    }

}
