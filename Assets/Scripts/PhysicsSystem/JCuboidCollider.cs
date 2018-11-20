using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionShapes;

public class JCuboidCollider : JCollider {

    public Vector3 Dimensions = new Vector3(1, 1, 1);
    Bounds bounds;

    [SerializeField]
    private Transform _testTransform;

    public override Bounds GenerateBounds()
    {
        bounds = new Bounds(Vector3.zero, Dimensions);
        return bounds;
    }

    public override Bounds GetBounds()
    {
        return bounds;
    }

    public override Vector3 GetClosestPoint(Vector3 fromPoint)
    {
        Vector3 localPoint = transform.InverseTransformPoint(fromPoint);

        localPoint.x = Mathf.Clamp(localPoint.x, bounds.min.x, bounds.max.x);
        localPoint.y = Mathf.Clamp(localPoint.y, bounds.min.y, bounds.max.y);
        localPoint.z = Mathf.Clamp(localPoint.z, bounds.min.z, bounds.max.z);

        return transform.TransformPoint(localPoint);

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

    private void OnDrawGizmos()
    {
        GenerateBounds();
        var oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, Dimensions);

        Gizmos.matrix = oldMatrix;

        Vector3 closestPoint = GetClosestPoint(_testTransform.position);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(closestPoint, 0.1f);
    }

}
