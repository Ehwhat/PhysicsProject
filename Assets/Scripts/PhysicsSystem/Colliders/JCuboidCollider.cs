using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionShapes;

public class JCuboidCollider : JMeshCollider {

    public Vector3 Dimensions = new Vector3(1, 1, 1);

    public Vector3 Extents
    {
        get { return Dimensions * 0.5f; }
    }

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

    Bounds bounds;

    [SerializeField]
    private Transform _testTransform;

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

    public override Vector3[] GetVertices()
    {
        Vector3[] verts = new Vector3[8];

        verts[0] = transform.position - (transform.right * Extents.x) - (transform.up * Extents.y) - (transform.forward * Extents.z); //new Vector3(GlobalMin.x, GlobalMin.y, GlobalMin.z);
        verts[1] = transform.position + (transform.right * Extents.x) + (transform.up * Extents.y) + (transform.forward * Extents.z); //new Vector3(GlobalMax.x, GlobalMax.y, GlobalMax.z);

        verts[2] = transform.position + (transform.right * Extents.x) - (transform.up * Extents.y) - (transform.forward * Extents.z); //new Vector3(GlobalMax.x, GlobalMin.y, GlobalMin.z);
        verts[3] = transform.position - (transform.right * Extents.x) + (transform.up * Extents.y) - (transform.forward * Extents.z); //new Vector3(GlobalMin.x, GlobalMax.y, GlobalMin.z);
        verts[4] = transform.position - (transform.right * Extents.x) - (transform.up * Extents.y) + (transform.forward * Extents.z); //new Vector3(GlobalMin.x, GlobalMin.y, GlobalMax.z);

        verts[5] = transform.position + (transform.right * Extents.x) + (transform.up * Extents.y) - (transform.forward * Extents.z); //new Vector3(GlobalMax.x, GlobalMax.y, GlobalMin.z);
        verts[6] = transform.position + (transform.right * Extents.x) - (transform.up * Extents.y) + (transform.forward * Extents.z); //new Vector3(GlobalMax.x, GlobalMin.y, GlobalMax.z);
        verts[7] = transform.position - (transform.right * Extents.x) + (transform.up * Extents.y) + (transform.forward * Extents.z); //new Vector3(GlobalMin.x, GlobalMax.y, GlobalMax.z);

        return verts;
    }

    public JLineSegment[] GetEdges()
    {
        Vector3[] verts = GetVertices();
        JLineSegment[] edges = new JLineSegment[12];

        edges[0] = new JLineSegment(verts[0], verts[5]);
        edges[1] = new JLineSegment(verts[0], verts[6]);
        edges[2] = new JLineSegment(verts[0], verts[7]);

        edges[3] = new JLineSegment(verts[1], verts[2]);
        edges[4] = new JLineSegment(verts[1], verts[3]);
        edges[5] = new JLineSegment(verts[1], verts[4]);

        edges[6] = new JLineSegment(verts[2], verts[6]);
        edges[7] = new JLineSegment(verts[2], verts[5]);

        edges[8] = new JLineSegment(verts[3], verts[5]);
        edges[9] = new JLineSegment(verts[3], verts[7]);

        edges[10] = new JLineSegment(verts[4], verts[6]);
        edges[11] = new JLineSegment(verts[4], verts[7]);

        return edges;
    }

    public Plane[] GetPlanes()
    {
        return new Plane[]
        {
            new Plane(transform.right, Extents.x),
            new Plane(-transform.right, Extents.x),

            new Plane(transform.up, Extents.y),
            new Plane(-transform.up, Extents.y),

            new Plane(transform.forward, Extents.z),
            new Plane(-transform.forward, Extents.z)

        };
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Bounds bounds = GenerateBounds();
        Gizmos.DrawWireCube(transform.position, bounds.size);

        Vector3[] verts = GetVertices();

        for (int i = 0; i < verts.Length; i++)
        {
            Gizmos.DrawSphere(verts[i], 0.1f);
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
