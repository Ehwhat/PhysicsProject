using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct JCollision {

    public bool valid;

    public List<JContact> contacts;

    public JCollider colliderA;

    public JCollider colliderB;

    public JCollision(JCollider colliderA, JCollider colliderB)
    {
        valid = false;
        this.colliderA = colliderA;
        this.colliderB = colliderB;
        contacts = new List<JContact>();
    }

    public void AddContact(Vector3 position, Vector3 normal, float depth)
    {
        contacts.Add(new JContact(position, normal, depth));
    }

    public void AddContact(JContact contact)
    {
        contacts.Add(contact);
    }

    public override bool Equals(object obj)
    {
        if(obj == null)
        {
            return false;
        }

        JCollision collision = (JCollision)obj;
        if(collision != null)
        {
            bool equal =
                (colliderA == collision.colliderA && colliderB == collision.colliderB) ||
                (colliderA == collision.colliderB && colliderB == collision.colliderA);
            return equal;
        }
        return false;
    }
    public static bool operator==(JCollision c1, JCollision c2)
    {
        if (object.ReferenceEquals(c1, null))
        {
            return object.ReferenceEquals(c2, null);
        }

        return c1.Equals(c2);
    }
    public static bool operator !=(JCollision c1, JCollision c2)
    {
        return !(c1 == c2);
    }
}
