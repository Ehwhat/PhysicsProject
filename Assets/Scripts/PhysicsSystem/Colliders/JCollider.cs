using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionShapes;

public abstract class JCollider : JColliderBase {

    public JRigidbody owningBody;

    public abstract Vector3 GetClosestPoint(Vector3 fromPoint, bool clampToEdge = false);
    public abstract bool IsPointInside(Vector3 point);

    public abstract bool Raycast(JRay ray, out JRaycastHit hitData);

    public abstract Matrix4x4 GetInverseTensor(float mass);

    public virtual void OnEnable()
    {
    }

    public virtual void OnDrawGizmos()
    {
        //Bounds bounds =  GetBounds();
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

}
