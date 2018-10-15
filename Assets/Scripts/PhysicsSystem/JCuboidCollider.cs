using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionShapes;

public class JCuboidCollider : JCollider {

    public Vector3 Dimensions = new Vector3(1, 1, 1);
    Bounds bounds;

    public override Bounds GenerateBounds()
    {
        bounds = new Bounds(Vector3.zero, Dimensions);
        return bounds;
    }

    public override Bounds GetBounds()
    {
        return bounds;
    }

    public override bool TestCollisionWith(JSphereCollider collider, out CollisionData collision)
    {
        collision = new CollisionData();
        return false;
    }

    public override bool TestCollisionWith(JPlaneCollider collider, out CollisionData collision)
    {
        return CollisionSolvers.CuboidVsPlaneCollision(new Cuboid(transform.position, Dimensions / 2), new Plane(collider.Normal, collider.transform.position), out collision);
    }

    public override bool TestCollisionWith(JCuboidCollider collider, out CollisionData collision)
    {
        return CollisionSolvers.CuboidVsCuboidCollision(new Cuboid(transform.position, Dimensions / 2), new Cuboid(collider.transform.position, collider.Dimensions / 2), out collision);
    }

}
