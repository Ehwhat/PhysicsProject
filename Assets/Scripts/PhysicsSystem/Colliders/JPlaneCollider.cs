using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionShapes;

public class JPlaneCollider : JCollider
{

    public Vector3 Normal
    {
        get { return transform.up; }
    }
    public float Distance
    {
        get { return Vector3.Dot(transform.position, Normal); }
    }
    private Bounds bounds;
    public Plane plane;

    [SerializeField]
    private Transform _testTransform;

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

    public override Vector3 GetClosestPoint(Vector3 fromPoint, bool clampToEdge = false)
    {
        float dotProduct = Vector3.Dot(Normal, fromPoint);
        float pointDistance = dotProduct - Distance;
        return fromPoint - Normal * pointDistance;
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        var oldMatix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        Gizmos.DrawWireCube(Vector3.zero, new Vector3(10000, 0, 10000));

        Gizmos.matrix = oldMatix;

        Vector3 closestPoint = GetClosestPoint(_testTransform.position);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(closestPoint, 0.1f);

    }

    public override bool IsPointInside(Vector3 point)
    {
        float dotProduct = Vector3.Dot(Normal, point);
        float pointDistance = dotProduct - Distance;
        return pointDistance <= 0;
    }

    public override bool Raycast(JRay ray, out JRaycastHit hitData)
    {
        hitData = new JRaycastHit();
        return false;
    }

    public override Matrix4x4 GetInverseTensor(float mass)
    {
        return Matrix4x4.identity;
    }
}
