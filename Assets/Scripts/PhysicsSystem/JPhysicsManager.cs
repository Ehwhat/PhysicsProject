using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPhysicsManager : MonoBehaviour {

    public int SolverIterations = 3;
    [Range(0,4)]
    public float epsilon = 0;
    private static List<JRigidbody> _bodies = new List<JRigidbody>();
    private Dictionary<CompareablePair<JRigidbody>,JCollision> collisions = new Dictionary<CompareablePair<JRigidbody>, JCollision>();

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
        List<JRaycastHit> hits = new List<JRaycastHit>();
        for (int i = 0; i < _bodies.Count; i++)
        {
            JCollider[] colliders = _bodies[i].GetColliders();
            for (int j = 0; j < colliders.Length; j++)
            {
                if(colliders[j].Raycast(ray,out hitData))
                {
                    hits.Add(hitData);
                }
            }
        }

        if (hits.Count > 0)
        {
            JRaycastHit bestHit = hits[0];
            for (int i = 1; i < hits.Count; i++)
            {
                if(hits[i].distance < bestHit.distance)
                {
                    bestHit = hits[i];
                }
            }
            hitData = bestHit;
            return true;
        }
        else
        {
            hitData = new JRaycastHit();
            hitData.hit = false;
            return false;
        }
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
        CheckAllBodiesForCollisions();
        SimulateBodies();
        for (int i = 0; i < SolverIterations; i++)
        {
            SolveFrameCollisions();
            for (int j = 0; j < collisions.Count; j++)
            {
                CorrectPositions(collisions[j]);
            }
        }
    }

    private void SimulateBodies()
    {
        for (int i = 0; i < _bodies.Count; i++)
        {
            _bodies[i].Simulate(Time.fixedDeltaTime);
        }
    }

    private void PreUpdateCollisions()
    {
        foreach (JCollision collision in collisions.Values)
        {

            JRigidbody bodyA = collision.colliderA.owningBody;
            JRigidbody bodyB = collision.colliderB.owningBody;

            foreach (JContact contact in collision.contacts)
            {
                Vector3 localContactPointA = contact.position - bodyA.transform.position;
                Vector3 localContactPointB = contact.position - bodyB.transform.position;

                float localNormalDotA = Vector3.Dot(localContactPointA, contact.normal);
                float localNormalDotB = Vector3.Dot(localContactPointB, contact.normal);

                contact.normalMass = bodyA._colliders[0].GetInverseTensor(bodyA.Mass).

            }
        }
    }

    private void SolveFrameCollisions()
    {
        for (int i = 0; i < collisions.Count; i++)
        {
            JCollision collision = collisions[i];
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

            Debug.DrawLine(bodyA.transform.position, collision.collisionPoints[0], Color.red);

            for (int c = 0; c < collision.collisionPoints.Count; c++)
            {
                Vector3 relativeContactPointA = collision.collisionPoints[c] - bodyA.transform.position;
                Vector3 relativeContactPointB = collision.collisionPoints[c] - bodyB.transform.position;

                Vector3 contactVelocity =
                    ((velocityB + Vector3.Cross(bodyB.AngularVelocity, relativeContactPointB)) -
                    (velocityA + Vector3.Cross(bodyA.AngularVelocity, relativeContactPointA)));
                float reactionForceMagnitude = Vector3.Dot(contactVelocity, normal);

                if (reactionForceMagnitude > 0)
                {
                    continue;
                }

                float numerator = (-(1.0f + epsilon) * reactionForceMagnitude);

                float d1 = (bodyA.GetInvMass() + bodyB.GetInvMass());
                Vector3 d2 = Vector3.Cross(invTensorA * Vector3.Cross(relativeContactPointA, normal), relativeContactPointA);
                Vector3 d3 = Vector3.Cross(invTensorB * Vector3.Cross(relativeContactPointB, normal), relativeContactPointB);
                float denominator = d1 + Vector3.Dot(normal, d2 + d3);

                float j = (denominator == 0) ? 0 : (numerator / denominator);
                j /= (float)collision.collisionPoints.Count;

                Vector3 impulseReactionForce = (j * normal);

                bodyA.SetVelocity(velocityA - impulseReactionForce * bodyA.GetInvMass());
                bodyB.SetVelocity(velocityB + impulseReactionForce * bodyB.GetInvMass());

                bodyA.AngularVelocity = bodyA.AngularVelocity - (Vector3)(invTensorA * Vector3.Cross(relativeContactPointA, impulseReactionForce));
                bodyB.AngularVelocity = bodyB.AngularVelocity + (Vector3)(invTensorB * Vector3.Cross(relativeContactPointB, impulseReactionForce));





                //    Vector3 tangent = (contactVelocity - (Vector3.Dot(contactVelocity, normal) * normal)).normalized;
                //    float staticFrictionAverage = PythSolver(bodyA.StaticFriction, bodyB.StaticFriction);
                //    float dynamicFrictionAverage = PythSolver(bodyA.DynamicFriction, bodyB.DynamicFriction);

                //    numerator = -Vector3.Dot(contactVelocity, tangent);

                //    d1 = (bodyA.GetInvMass() + bodyB.GetInvMass());
                //    d2 = Vector3.Cross(invTensorA * Vector3.Cross(relativeContactPointA, tangent), relativeContactPointA);
                //    d3 = Vector3.Cross(invTensorB * Vector3.Cross(relativeContactPointB, tangent), relativeContactPointB);
                //    denominator = d1 + Vector3.Dot(tangent, d2 + d3);

                //    if (denominator == 0)
                //    {
                //        continue;
                //    }



                //    float jt = (numerator / denominator) / (float)collision.collisionPoints.Count;

                //    if (jt <= 0)
                //    {
                //        continue;
                //    }

                //    Debug.Log("test");


                //    if (jt > j * dynamicFrictionAverage)
                //    {
                //        jt = j * dynamicFrictionAverage;
                //    }
                //    else if (jt < -j * dynamicFrictionAverage)
                //    {
                //        jt = -j * dynamicFrictionAverage;
                //    }

                //    Vector3 tangentImpulse = tangent * jt;

                //    bodyA.SetVelocity(bodyA.Velocity - (tangentImpulse * bodyA.GetInvMass()));
                //    bodyB.SetVelocity(bodyB.Velocity + (tangentImpulse * bodyB.GetInvMass()));

                //    bodyA.AngularVelocity = bodyA.AngularVelocity - (Vector3)(invTensorA * Vector3.Cross(relativeContactPointA, impulseReactionForce));
                //    bodyB.AngularVelocity = bodyB.AngularVelocity + (Vector3)(invTensorB * Vector3.Cross(relativeContactPointB, impulseReactionForce));
                //}

                //if(Mathf.Abs(bodyA.Velocity.magnitude) < 0.1f)
                //{
                //    bodyA.SetVelocity(Vector3.zero);
                //}

                //if (Mathf.Abs(bodyB.Velocity.magnitude) < 0.1f)
                //{
                //    bodyB.SetVelocity(Vector3.zero);
                //}

                //if (Mathf.Abs(bodyA.AngularVelocity.magnitude) < 0.1f)
                //{
                //    bodyA.AngularVelocity = (Vector3.zero);
                //}

                //if (Mathf.Abs(bodyB.AngularVelocity.magnitude) < 0.1f)
                //{
                //    bodyB.AngularVelocity = (Vector3.zero);
                //}

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

        Vector3 correction = (Mathf.Max(collision.collisionDepth - slop, 0.0f) / (bodyA.Mass + bodyB.Mass)) * percent * collision.collisionNormal;
        if (!bodyA._isKinematic)
        {
            bodyA.transform.position -= bodyA.Mass * correction;
        }
        if (!bodyB._isKinematic) {
            bodyB.transform.position += bodyB.Mass * correction;
        }
        //bodyA.ApplyForce(bodyA.Mass * correction * 200);
        //bodyB.ApplyForce(-bodyB.Mass * correction * 200);

    }

    private void CheckAllBodiesForCollisions()
    {
        for (int i = 0; i < _bodies.Count; i++)
        {
            for (int j = i+1; j < _bodies.Count; j++)
            {

                if(_bodies[i].GetInvMass() == 0 && _bodies[i].GetInvMass() == 0)
                {
                    continue;
                }

                CompareablePair<JRigidbody> pair = new CompareablePair<JRigidbody>(_bodies[i], _bodies[j]);
                JCollision collision;
                if (GetBodyCollisions(_bodies[i], _bodies[j], out collision))
                {
                    if (collisions.ContainsKey(pair))
                    {
                        
                    }
                    else
                    {
                        collisions.Add(pair, collision);
                    }
                }
                else
                {
                    collisions.Remove(pair);
                }
            }
        }
    }

    private bool GetBodyCollisions(JRigidbody bodyA, JRigidbody bodyB, out JCollision collision)
    {
        if(CheckIfBoundsCollide(bodyA.GetBounds(), bodyB.GetBounds()))
        {
            return GetBodyCollisionsWithColliders(bodyA, bodyB, out collision);
        }
        collision = new JCollision();
        return false;
    }

    private bool GetBodyCollisionsWithColliders(JRigidbody bodyA, JRigidbody bodyB, out JCollision collision)
    {
        JCollider[] collidersA = bodyA.GetColliders();
        JCollider[] collidersB = bodyB.GetColliders();
        if (JCollisionSolver.SolveCollision(collidersA[0], collidersB[0], out collision))
        {
            return true;
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
