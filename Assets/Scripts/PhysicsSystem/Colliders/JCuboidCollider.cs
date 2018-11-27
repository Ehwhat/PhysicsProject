using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionShapes;

public class JCuboidCollider : JMeshCollider {

    public Vector3 LocalMin
    {
        get { return -Dimensions * 0.5f; }
    }

    public Vector3 LocalMax
    {
        get { return Dimensions * 0.5f; }
    }

    public Vector3 GlobalMin
    {
        get { return transform.TransformPoint(LocalMin); }
    }

    public Vector3 GlobalMax
    {
        get { return transform.TransformPoint(LocalMax); }
    }

    [SerializeField]
    public Vector3 Dimensions;
    Bounds bounds;

    [SerializeField]
    private Transform _testTransform;

    private Matrix4x4 scaleMatrix;
    private Vector3 Extents;

    private void Start()
    {
        scaleMatrix = Matrix4x4.Scale(transform.localScale);
        Extents = Dimensions * 0.5f;
    }

    public override Bounds GenerateBounds()
    {
        Vector3 MinMaxDimensions = new Vector3(
            GetMeshSegmentOnAxis(this, Vector3.left).Length,
            GetMeshSegmentOnAxis(this, Vector3.up).Length,
            GetMeshSegmentOnAxis(this, Vector3.forward).Length
            );
        bounds = new Bounds(Vector3.zero, MinMaxDimensions);
        return bounds;
    }

    public override Bounds GetBounds()
    {
        return GenerateBounds();
    }

    public override Vector3 GetClosestPoint(Vector3 fromPoint, bool clampToEdge = false)
    {
        Vector3 localPoint = transform.InverseTransformPoint(fromPoint);

        if (true)
        {
            if(Mathf.Abs(localPoint.x) >= Mathf.Abs(localPoint.y) && Mathf.Abs(localPoint.x) >= Mathf.Abs(localPoint.z) && localPoint.x < LocalMax.x && localPoint.x > LocalMin.x)
            {
                localPoint.x = LocalMax.x * Mathf.Sign(localPoint.x);
            }else if(Mathf.Abs(localPoint.y) >= Mathf.Abs(localPoint.x) && Mathf.Abs(localPoint.y) >= Mathf.Abs(localPoint.z) && localPoint.y < LocalMax.y && localPoint.y > LocalMin.y)
            {
                localPoint.y = LocalMax.y * Mathf.Sign(localPoint.y);
            }
            else if (Mathf.Abs(localPoint.z) >= Mathf.Abs(localPoint.x) && Mathf.Abs(localPoint.z) >= Mathf.Abs(localPoint.y) && localPoint.z < LocalMax.z && localPoint.z > LocalMin.z)
            {
                localPoint.z = LocalMax.z * Mathf.Sign(localPoint.z);
            }
        }

        localPoint.x = Mathf.Clamp(localPoint.x, LocalMin.x, LocalMax.x);
        localPoint.y = Mathf.Clamp(localPoint.y, LocalMin.y, LocalMax.y);
        localPoint.z = Mathf.Clamp(localPoint.z, LocalMin.z, LocalMax.z);

        return transform.TransformPoint(localPoint);

    }

    public override bool IsPointInside(Vector3 point)
    {
        Vector3 localPoint = transform.InverseTransformPoint(point);

        return
            localPoint.x >= LocalMin.x &&
            localPoint.y >= LocalMin.y &&
            localPoint.z >= LocalMin.z &&

            localPoint.x <= LocalMax.x &&
            localPoint.y <= LocalMax.y &&
            localPoint.z <= LocalMax.z;

    }

    public override Matrix4x4 GetInverseTensor(float mass)
    {
        if(mass <= 0)
        {
            return Matrix4x4.zero;
        }

        Vector3 DimensionSquared = new Vector3(Dimensions.x * Dimensions.x, Dimensions.y * Dimensions.y, Dimensions.z * Dimensions.z);
        float multiplier = (1.0f / 12.0f);

        float xComponent = (DimensionSquared.y + DimensionSquared.z) * mass * multiplier;
        float yComponent = (DimensionSquared.x + DimensionSquared.z) * mass * multiplier;
        float zComponent = (DimensionSquared.x + DimensionSquared.y) * mass * multiplier;

        return new Matrix4x4(
            new Vector4(xComponent, 0, 0, 0),
            new Vector4(0, yComponent, 0, 0),
            new Vector4(0, 0, zComponent, 0),
            new Vector4(0, 0, 0, 1)
            ).inverse;

    }

    private void OnDrawGizmos()
    {
        //GenerateBounds();
        //var oldMatrix = Gizmos.matrix;
        //Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        //Gizmos.color = Color.green;
        //Gizmos.DrawWireCube(Vector3.zero, Dimensions);

        //Gizmos.matrix = oldMatrix;

        //Vector3 closestPoint = GetClosestPoint(_testTransform.position);
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(closestPoint, 0.1f);
    }

    public override Vector3[] GetVertices()
    {
        Vector3[] verts = new Vector3[8];

        Vector3 position = transform.position;
        Vector3 up = transform.up;
        Vector3 right = transform.right;
        Vector3 forward = transform.forward;

        verts[0] = position + (right * Extents.x) + (up * Extents.y) + (forward * Extents.z);
        verts[1] = position - (right * Extents.x) + (up * Extents.y) + (forward * Extents.z);
        verts[2] = position + (right * Extents.x) - (up * Extents.y) + (forward * Extents.z);
        verts[3] = position + (right * Extents.x) + (up * Extents.y) - (forward * Extents.z);
        verts[4] = position - (right * Extents.x) - (up * Extents.y) - (forward * Extents.z);
        verts[5] = position + (right * Extents.x) - (up * Extents.y) - (forward * Extents.z);
        verts[6] = position - (right * Extents.x) + (up * Extents.y) - (forward * Extents.z);
        verts[7] = position - (right * Extents.x) - (up * Extents.y) + (forward * Extents.z);

        return verts;
    }

    public JLineSegment[] GetEdges()
    {
        Vector3[] verts = GetVertices();
        JLineSegment[] edges = new JLineSegment[12];

        edges[0] = new JLineSegment(verts[6], verts[1]);
        edges[1] = new JLineSegment(verts[6], verts[3]);
        edges[2] = new JLineSegment(verts[6], verts[4]);

        edges[3] = new JLineSegment(verts[2], verts[7]);
        edges[4] = new JLineSegment(verts[2], verts[5]);
        edges[5] = new JLineSegment(verts[2], verts[0]);

        edges[6] = new JLineSegment(verts[0], verts[1]);
        edges[7] = new JLineSegment(verts[0], verts[3]);

        edges[8] = new JLineSegment(verts[7], verts[1]);
        edges[9] = new JLineSegment(verts[7], verts[4]);

        edges[10] = new JLineSegment(verts[4], verts[5]);
        edges[11] = new JLineSegment(verts[5], verts[3]);

        return edges;
    }

    public Plane[] GetPlanes()
    {
        Vector3 pos = transform.position;
        return new Plane[]
        {
            new Plane(transform.right, Vector3.Dot(transform.right,pos + transform.right * Extents.x)),
            new Plane(transform.right * -1, -Vector3.Dot(transform.right,pos - transform.right * Extents.x)),

            new Plane(transform.up, Vector3.Dot(transform.up,pos + transform.up * Extents.y)),
            new Plane(transform.up * -1, -Vector3.Dot(transform.up,pos - transform.up * Extents.y)),

            new Plane(transform.forward, Vector3.Dot(transform.forward,pos + transform.forward * Extents.z)),
            new Plane(transform.forward * -1, -Vector3.Dot(transform.forward,pos - transform.forward * Extents.z))

        };
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Bounds bounds = GenerateBounds();

        JLineSegment[] edges = GetEdges();

        for (int i = 0; i < edges.Length; i++)
        {
            Gizmos.DrawLine(edges[i].Start, edges[i].End);
        }


        Vector3[] verts = GetVertices();

        for (int i = 0; i < verts.Length; i++)
        {
            Gizmos.DrawSphere(verts[i], 0.1f);
            
        }

        Plane[] planes = GetPlanes();

        for (int i = 0; i < planes.Length; i++)
        {
            Gizmos.DrawRay(transform.position, planes[i].normal);
        }



        Gizmos.color = Color.cyan;

        Gizmos.DrawSphere(GlobalMax, 0.2f);

    }

    public override bool Raycast(JRay ray, out JRaycastHit hitData)
    {
        hitData = new JRaycastHit();
        hitData.hit = false;

        Vector3 directionToCuboid = transform.position - ray.origin;

        Vector3 rayDirectionDots = new Vector3(
            Vector3.Dot(transform.right, ray.direction) + 0.0001f, // Super hacky cheat to make sure we don't divide by 0 later
            Vector3.Dot(transform.up, ray.direction) + 0.0001f,
            Vector3.Dot(transform.forward, ray.direction) + 0.0001f
            );
        Vector3 cuboidDirectionDots = new Vector3(
            Vector3.Dot(transform.right, directionToCuboid),
            Vector3.Dot(transform.up, directionToCuboid),
            Vector3.Dot(transform.forward, directionToCuboid)
            );

        float[] distances = new float[6];
        for (int i = 0; i < 3; i++)
        {
            /*if(-cuboidDirectionDots[i] - Extents[i] > 0 || -cuboidDirectionDots[i] + Extents[i] < 0)
            {
                return false;
            }*/
            distances[i * 2 + 0] = (cuboidDirectionDots[i] + Extents[i]) / rayDirectionDots[i];
            distances[i * 2 + 1] = (cuboidDirectionDots[i] - Extents[i]) / rayDirectionDots[i];
        }

        float largestMinimumDistance = Mathf.Max(
                Mathf.Min(distances[0], distances[1]),
                Mathf.Min(distances[2], distances[3]),
                Mathf.Min(distances[4], distances[5])
            );

        float smallestMaximumDistance = Mathf.Min(
                Mathf.Max(distances[0], distances[1]),
                Mathf.Max(distances[2], distances[3]),
                Mathf.Max(distances[4], distances[5])
            );

        if(smallestMaximumDistance < 0) // This means our best point is behind the origin, and that doesn't make sense, so we'll cull it.
        {
            return false;
        }
        if(largestMinimumDistance > smallestMaximumDistance)
        {
            return false;
        }

        hitData.hit = true;
        hitData.hitCollider = this;
        hitData.origin = ray.origin;

        if(largestMinimumDistance < 0)
        {
            hitData.distance = smallestMaximumDistance;
        }
        else
        {
            hitData.distance = largestMinimumDistance;
        }

        hitData.hitPoint = ray.origin + (ray.direction * hitData.distance);
        return true;

    }
}
