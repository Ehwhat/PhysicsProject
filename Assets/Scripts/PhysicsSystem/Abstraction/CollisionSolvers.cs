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

    public static bool CuboidVsPlaneCollision(Cuboid a, Plane b, out CollisionData collision)
    {
        Vector3 planeNormal = b.normal;
        Vector3 extents = a.extents;
        Vector3 center = a.center;

        float fRadius = Mathf.Abs(planeNormal.x * extents.x) + Mathf.Abs(planeNormal.y * extents.y) + Mathf.Abs(planeNormal.z * extents.z);

        Sphere sphere = new Sphere(center, fRadius);
        return SphereVsPlaneCollision(sphere, b, out collision);
    }

    public static bool CuboidVsCuboidCollision(Cuboid a, Cuboid b, out CollisionData collision)
    {
        //Vector3 direction = (b.center - a.center).normalized;

        //float fRadiusA = Mathf.Abs(direction.x * a.extents.x) + Mathf.Abs(direction.y * a.extents.y) + Mathf.Abs(direction.z * a.extents.z);
        //float fRadiusB = Mathf.Abs(direction.x * b.extents.x) + Mathf.Abs(direction.y * b.extents.y) + Mathf.Abs(direction.z * b.extents.z);

        //Sphere sphereA = new Sphere(a.center, fRadiusA);
        //Sphere sphereB = new Sphere(b.center, fRadiusB);
        //return SphereVsSphereCollision(sphereA, sphereB, out collision);

        //Vector3 minA = a.center - a.extents;
        //Vector3 maxA = a.center + a.extents;

        //Vector3 minB = b.center - b.extents;
        //Vector3 maxB = b.center + b.extents;

        //Vector3 sizeA = a.extents * 2;
        //Vector3 sizeB = b.extents * 2;
        //Vector3 sizeAll = sizeA + sizeB;

        //Vector3 differenceA = minA - minB;
        //float depth = 0;
        //Vector3 normal = Vector3.zero;

        //if(differenceA.x < sizeA.x)
        //{

        //}

        Vector3 extentsAll = (a.extents) + (b.extents);
        Vector3 difference = b.center - a.center;

        float xDifference = Mathf.Abs(Vector3.Dot(difference, Vector3.right));
        float yDifference = Mathf.Abs(Vector3.Dot(difference, Vector3.up));
        float zDifference = Mathf.Abs(Vector3.Dot(difference, Vector3.forward));

        if (xDifference < extentsAll.x && xDifference > yDifference && xDifference > zDifference)
        {
            collision.collisionNormal = Vector3.right * Mathf.Sign(-Vector3.Dot(difference, Vector3.right));
            collision.collisionDepth = extentsAll.x - xDifference;
            collision.collisionPoint = a.center + Vector3.right * xDifference;
            return true;

        }
        if (yDifference < extentsAll.y && yDifference > xDifference && yDifference > zDifference)
        {
            collision.collisionNormal = Vector3.up * Mathf.Sign(-Vector3.Dot(difference, Vector3.up));
            collision.collisionDepth = extentsAll.y - yDifference;
            collision.collisionPoint = a.center + Vector3.up * yDifference;
            return true;
        }
        if (zDifference < extentsAll.z && zDifference > xDifference && zDifference > yDifference)
        {
            collision.collisionNormal = Vector3.forward * Mathf.Sign(-Vector3.Dot(difference, Vector3.forward));
            collision.collisionDepth = extentsAll.z - zDifference;
            collision.collisionPoint = a.center + Vector3.forward * zDifference;
            return true;
        }
        collision = new CollisionData();
        return false;


    }

}
