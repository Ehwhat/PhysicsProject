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

    public override bool Raycast(JRay ray, out JRaycastHit hitData)
    {
        hitData = new JRaycastHit();
        Vector3 directionToSphere = transform.position - ray.origin;
        float radiusSquared = Radius * Radius;

        float dotProduct = Vector3.Dot(directionToSphere, ray.direction);
        float offset = directionToSphere.sqrMagnitude - (dotProduct * dotProduct);
        float lengthFromSphereSurfaceToOffset = Mathf.Sqrt(radiusSquared - offset);

        hitData.hit = false;
        hitData.origin = ray.origin;

        if(radiusSquared - offset < 0)
        {
            return false;
        }

        hitData.hit = true;
        hitData.hitCollider = this;

        if(directionToSphere.sqrMagnitude < radiusSquared) // If it's inside the sphere
        {
            hitData.distance = dotProduct + lengthFromSphereSurfaceToOffset;
        }
        else
        {
            hitData.distance = dotProduct - lengthFromSphereSurfaceToOffset;
        }

        hitData.hitPoint = ray.origin + (ray.direction * hitData.distance);

        return true;

    }

    public override Matrix4x4 GetInverseTensor(float mass)
    {
        float radiusSquared = Radius * Radius;
        float multiplier = (2.0f / 5f);

        float xComponent = radiusSquared * mass * multiplier;
        float yComponent = radiusSquared * mass * multiplier;
        float zComponent = radiusSquared * mass * multiplier;

        return new Matrix4x4(
            new Vector4(xComponent, 0, 0, 0),
            new Vector4(0, yComponent, 0, 0),
            new Vector4(0, 0, zComponent, 0),
            new Vector4(0, 0, 0, 1)
            ).inverse;

    }
}
