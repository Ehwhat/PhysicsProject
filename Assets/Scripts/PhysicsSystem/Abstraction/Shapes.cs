using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollisionShapes
{
    public struct Sphere
    {
        public Vector3 center;
        public float radius;
        public Sphere(Vector3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }
    }
    //Using the UnityEngine definition of a plane, due to it just working fine.
    public struct Cuboid
    {
        public Vector3 center;
        public Vector3 extents;
        public Cuboid(Vector3 center, Vector3 extents)
        {
            this.center = center;
            this.extents = extents;
        }
    }
}
