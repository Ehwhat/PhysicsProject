using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionShapes;

public static class CollisionSolvers {
    
    public static bool SphereVsSphereCollision(Sphere a, Sphere b, out CollisionData collision)
    {
        float distance = Vector3.Distance(a.center, b.center);
        float totalRadius = a.radius + b.radius;
        if (distance <= totalRadius)
        {
            Vector3 direction = (b.center - a.center).normalized;
            Vector3 collisionPoint = direction * a.radius;
            Vector3 collisionNormal = -direction;
            float depth = totalRadius - distance;
            collision = new CollisionData(collisionPoint, collisionNormal, depth);
            return true;
        }
        collision = new CollisionData();
        return false;
    }
    public static bool SphereVsPlaneCollision(Sphere a, Plane b, out CollisionData collision)
    {
        collision = new CollisionData();
        if (b.normal.sqrMagnitude == 0)
        {
            return false;
        }
        Vector3 direction = (b.ClosestPointOnPlane(a.center) - a.center).normalized;

        float distance = b.GetDistanceToPoint(a.center);
        if (distance <= a.radius)
        {

            float depth = a.radius - distance;
            Vector3 collisionPoint = direction * depth;
            collision = new CollisionData(collisionPoint, b.normal, depth);
            return true;
        }
        return false;
    }

    public static bool CubiodVsPlaneCollision(Cuboid a, Plane b, out CollisionData collision)
    {
        Vector3 planeNormal = b.normal;
        Vector3 extents = a.extents;
        Vector3 center = a.center;

        float fRadius = Mathf.Abs(planeNormal.x * extents.x) + Mathf.Abs(planeNormal.y * extents.y) + Mathf.Abs(planeNormal.z * extents.z);

        Sphere sphere = new Sphere(center, fRadius);
        return SphereVsPlaneCollision(sphere, b, out collision);
    }

}
