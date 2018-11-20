using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionShapes;

public abstract class JCollider : JColliderBase {

    public JRigidbody owningBody;

    public bool TestCollisionWith(JCollider collider, out JCollision collision)
    {
        CollisionData data;
        if(collider is JSphereCollider)
        {
            if(TestCollisionWith((JSphereCollider)collider, out data))
            {
                collision = new JCollision(data, this, collider);
                return true;
            }
        }
        if (collider is JCuboidCollider)
        {
            if (TestCollisionWith((JCuboidCollider)collider, out data))
            {
                collision = new JCollision(data, this, collider);
                return true;
            }
        }
        if (collider is JPlaneCollider)
        {
            if (TestCollisionWith((JPlaneCollider)collider, out data))
            {
                collision = new JCollision(data, this, collider);
                return true;
            }
        }
        collision = null; ;
        return false;
    }

    public abstract bool TestCollisionWith(JSphereCollider collider, out CollisionData collision);
    public abstract bool TestCollisionWith(JPlaneCollider collider, out CollisionData collision);
    public abstract bool TestCollisionWith(JCuboidCollider collider, out CollisionData collision);

    public abstract Vector3 GetClosestPoint(Vector3 fromPoint);

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
