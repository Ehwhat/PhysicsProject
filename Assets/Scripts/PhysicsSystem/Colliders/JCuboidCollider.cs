using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionShapes;

public class JCuboidCollider : JCollider {

    public Vector3 Dimensions = new Vector3(1, 1, 1);

    public Vector3 Min
    {
        get { return -Dimensions * 0.5f; }
    }

    public Vector3 Max
    {
        get { return Dimensions * 0.5f; }
    }

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

    public override Vector3 GetClosestPoint(Vector3 fromPoint, bool clampToEdge = false)
    {
        Vector3 localPoint = transform.InverseTransformPoint(fromPoint);

        if (true)
        {
            if(Mathf.Abs(localPoint.x) >= Mathf.Abs(localPoint.y) && Mathf.Abs(localPoint.x) >= Mathf.Abs(localPoint.z) && localPoint.x < Max.x && localPoint.x > Min.x)
            {
                localPoint.x = Max.x * Mathf.Sign(localPoint.x);
            }else if(Mathf.Abs(localPoint.y) >= Mathf.Abs(localPoint.x) && Mathf.Abs(localPoint.y) >= Mathf.Abs(localPoint.z) && localPoint.y < Max.y && localPoint.y > Min.y)
            {
                localPoint.y = Max.y * Mathf.Sign(localPoint.y);
            }
            else if (Mathf.Abs(localPoint.z) >= Mathf.Abs(localPoint.x) && Mathf.Abs(localPoint.z) >= Mathf.Abs(localPoint.y) && localPoint.z < Max.z && localPoint.z > Min.z)
            {
                localPoint.z = Max.z * Mathf.Sign(localPoint.z);
            }
        }

        localPoint.x = Mathf.Clamp(localPoint.x, Min.x, Max.x);
        localPoint.y = Mathf.Clamp(localPoint.y, Min.y, Max.y);
        localPoint.z = Mathf.Clamp(localPoint.z, Min.z, Max.z);

        return transform.TransformPoint(localPoint);

    }

    public override bool IsPointInside(Vector3 point)
    {
        Vector3 localPoint = transform.InverseTransformPoint(point);

        return
            localPoint.x >= Min.x &&
            localPoint.y >= Min.y &&
            localPoint.z >= Min.z &&

            localPoint.x <= Max.x &&
            localPoint.y <= Max.y &&
            localPoint.z <= Max.z;

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
