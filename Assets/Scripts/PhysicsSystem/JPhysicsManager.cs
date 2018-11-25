using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPhysicsManager : MonoBehaviour {

    public int SolverIterations = 3;
    [Range(0,4)]
    public float epsilon = 0;
    private static List<JRigidbody> _bodies = new List<JRigidbody>();
    private List<JCollision> _frameCollisions = new List<JCollision>();

    private bool wasHit = false;
    private Vector3 testPoint;

    public static void RegisterBody(JRigidbody body)
    {
        _bodies.Add(body);
        Debug.Log("Registered " + body.name);
    }

    public static void DeregisterBody(JRigidbody body)
    {
        _bodies.Remove(body);
    }

    public bool Raycast(JRay ray, out JRaycastHit hitData)
    {
        for (int i = 0; i < _bodies.Count; i++)
        {
            JCollider[] colliders = _bodies[i].GetColliders();
            for (int j = 0; j < colliders.Length; j++)
            {
                if(colliders[j].Raycast(ray,out hitData))
                {
                    return true;
                }
            }
        }
        hitData = new JRaycastHit();
        hitData.hit = false;
        return false;
    }

    private void Update()
    {
        JRaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Raycast(new JRay(ray.origin, ray.direction), out hit))
        {
            wasHit = true;
            testPoint = hit.hitPoint;
            if (Input.GetMouseButtonDown(0))
            {
                hit.hitCollider.owningBody.ApplyAngularImpulse(hit.hitPoint,ray.direction * 10);
            }
        }
        else
        {
            wasHit = false;
        }
    }

    private void FixedUpdate()
    {
        SimulateBodies();
        for (int i = 0; i < SolverIterations; i++)
        {
            CheckAllBodiesForCollisions();
            SolveFrameCollisions();
        }
    }

    private void SimulateBodies()
    {
        for (int i = 0; i < _bodies.Count; i++)
        {
            _bodies[i].Simulate(Time.fixedDeltaTime);
        }
    }

    private void SolveFrameCollisions()
    {
        for (int i = 0; i < _frameCollisions.Count; i++)
        {
            JCollision collision = _frameCollisions[i];
            JRigidbody bodyA = collision.colliderA.owningBody;
            JRigidbody bodyB = collision.colliderB.owningBody;
            if(bodyA == null || bodyB == null)
            {
                continue;
            }

            Vector3 normal = collision.collisionNormal.normalized;
            Vector3 velocityA = bodyA.Velocity;
            Vector3 velocityB = bodyB.Velocity;
            Matrix4x4 invTensorA = bodyA._colliders[0].GetInverseTensor(bodyA.Mass);
            Matrix4x4 invTensorB = bodyB._colliders[0].GetInverseTensor(bodyB.Mass);

            

            Debug.Log(collision.collisionPoints.Count);

            for (int c = 0; c < collision.collisionPoints.Count; c++)
            {

                Vector3 relativeContactPointA = collision.collisionPoints[c] - bodyA.transform.position;
                Vector3 relativeContactPointB = collision.collisionPoints[c] - bodyB.transform.position;

                Vector3 contactVelocity = 
                    ((velocityB + Vector3.Cross(bodyB.AngularVelocity, relativeContactPointB)) -
                    (velocityA + Vector3.Cross(bodyA.AngularVelocity, relativeContactPointA)));
                float reactionForceMagnitude = Vector3.Dot(contactVelocity, normal);

                if (reactionForceMagnitude < 0.05)
                {
                    continue;
                }

                Vector3 tangent = (contactVelocity - Vector3.Dot(contactVelocity, normal) * normal).normalized;
                float staticFrictionAverage = PythSolver(bodyA.StaticFriction, bodyB.StaticFriction);
                float dynamicFrictionAverage = PythSolver(bodyA.DynamicFriction, bodyB.DynamicFriction);

                float numerator = (-(1.0f + epsilon) * reactionForceMagnitude);

                float d1 = (bodyA.GetInvMass() + bodyB.GetInvMass());
                Vector3 d2 = Vector3.Cross(invTensorA * Vector3.Cross(relativeContactPointA, normal), relativeContactPointA);
                Vector3 d3 = Vector3.Cross(invTensorB * Vector3.Cross(relativeContactPointB, normal), relativeContactPointB);
                float denominator = d1 + Vector3.Dot(normal, d2 + d3);

                float jt = -Vector3.Dot(contactVelocity, tangent) / (bodyA.GetInvMass() + bodyB.GetInvMass());
            
                float j = (denominator == 0) ? 0 : (numerator / denominator)/(float)collision.collisionPoints.Count;

                Vector3 impulseReactionForce = (j * normal);

                //Vector3 frictionReactionForce;
                //if (Mathf.Abs(jt) < j * staticFrictionAverage)
                //{
                //    frictionReactionForce = jt * tangent;
                //}
                //else
                //{
                //    frictionReactionForce = -j * tangent * dynamicFrictionAverage;
                //}

                Vector3 bodyAForce = (impulseReactionForce) * bodyA.GetInvMass();
                Vector3 bodyBForce = (impulseReactionForce) * bodyB.GetInvMass();

                bodyA.SetVelocity(velocityA - bodyAForce);
                bodyB.SetVelocity(velocityB + bodyBForce);

                bodyA.AngularVelocity = bodyA.AngularVelocity - (Vector3)(invTensorA * Vector3.Cross(relativeContactPointA, (impulseReactionForce)));
                bodyB.AngularVelocity = bodyB.AngularVelocity - (Vector3)(invTensorB * Vector3.Cross(relativeContactPointB, (impulseReactionForce)));

                CorrectPositions(collision);
            }

            if(Mathf.Abs(bodyA.Velocity.magnitude) < 0.1f)
            {
                bodyA.SetVelocity(Vector3.zero);
            }

            if (Mathf.Abs(bodyB.Velocity.magnitude) < 0.1f)
            {
                bodyB.SetVelocity(Vector3.zero);
            }

            if (Mathf.Abs(bodyA.AngularVelocity.magnitude) < 0.1f)
            {
                bodyA.AngularVelocity = (Vector3.zero);
            }

            if (Mathf.Abs(bodyB.AngularVelocity.magnitude) < 0.1f)
            {
                bodyB.AngularVelocity = (Vector3.zero);
            }

        }
    }

    private float PythSolver(float a, float b)
    {
        return Mathf.Sqrt(a * a + b * b);
    }

    private void CorrectPositions(JCollision collision)
    {
        const float slop = 0.01f;
        const float percent = 0.6f;

        JRigidbody bodyA = collision.colliderA.owningBody;
        JRigidbody bodyB = collision.colliderB.owningBody;

        Vector3 correction = (Mathf.Max(collision.collisionDepth - slop, 0.0f) / (bodyA.Mass + bodyB.Mass)) * percent * -collision.collisionNormal;
        if(!bodyA._isKinematic)
            bodyA.transform.position -= bodyA.Mass * correction;
        if (!bodyB._isKinematic)
            bodyB.transform.position += bodyB.Mass * correction;
        //bodyA.ApplyForce(bodyA.Mass * correction * 200);
        //bodyB.ApplyForce(-bodyB.Mass * correction * 200);

    }

    private void CheckAllBodiesForCollisions()
    {
        HashSet<CompareablePair<JRigidbody>> checkedPairs = new HashSet<CompareablePair<JRigidbody>>();
        _frameCollisions = new List<JCollision>();
        for (int i = 0; i < _bodies.Count; i++)
        {
            for (int j = 0; j < _bodies.Count; j++)
            {
                if (i == j) continue;
                CompareablePair<JRigidbody> pair = new CompareablePair<JRigidbody>(_bodies[i], _bodies[j]);
                if (!checkedPairs.Contains(pair))
                {
                    GetBodyCollisions(_bodies[i], _bodies[j], ref _frameCollisions);
                    checkedPairs.Add(pair);
                }
            }
        }
    }

    private bool GetBodyCollisions(JRigidbody bodyA, JRigidbody bodyB, ref List<JCollision> collisions)
    {
        if(CheckIfBoundsCollide(bodyA.GetBounds(), bodyB.GetBounds()))
        {
            return GetBodyCollisionsWithColliders(bodyA, bodyB, ref collisions);
        }
        return false;
    }

    private bool GetBodyCollisionsWithColliders(JRigidbody bodyA, JRigidbody bodyB, ref List<JCollision> collisions)
    {
        JCollider[] collidersA = bodyA.GetColliders();
        JCollider[] collidersB = bodyB.GetColliders();
        for (int i = 0; i < collidersA.Length; i++)
        {
            
            for (int j = 0; j < collidersB.Length; j++)
            {
                JCollision collision;
                if (JCollisionSolver.SolveCollision(collidersA[i], collidersB[j], out collision))
                {
                    collisions.Add(collision);
                }
            }
        }
        return false ;
    }

    private bool CheckIfBoundsCollide(Bounds boundA, Bounds boundB)
    {
        return boundA.Intersects(boundB);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (wasHit)
        {
            Gizmos.DrawSphere(testPoint, 0.1f);
        }
    }

}
