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
                hit.hitCollider.owningBody.ApplyImpulse(ray.direction * 10);
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
            if(bodyA == null && bodyB == null)
            {
                continue;
            }

            Vector3 normal = collision.collisionNormal;
            Vector3 velocityA = bodyA.Velocity;
            Vector3 velocityB = bodyB.Velocity;

            if(velocityA.sqrMagnitude == 0 && velocityB.sqrMagnitude == 0)
            {
                if (!bodyA._isKinematic)
                {
                    velocityA = normal;
                }else if (!bodyB._isKinematic)
                {
                    velocityB = normal;
                }
            }

            Vector3 contactVelocity = velocityB - velocityA;
            float reactionForceMagnitude = Vector3.Dot(contactVelocity, normal);

            if (reactionForceMagnitude < 0.05)
            {
                continue;
            }

            Vector3 tangent = (contactVelocity - Vector3.Dot(contactVelocity, normal) * normal).normalized;
            float staticFrictionAverage = PythSolver(bodyA.StaticFriction, bodyB.StaticFriction);
            float dynamicFrictionAverage = PythSolver(bodyA.DynamicFriction, bodyB.DynamicFriction);


            float jt = -Vector3.Dot(contactVelocity, tangent) / (1 / bodyA.Mass + 1 / bodyB.Mass);
            float j = -(1.0f + epsilon) * reactionForceMagnitude;
            
            Vector3 impulseReactionForce = (j*normal) / (bodyA.Mass + bodyB.Mass);

            Vector3 frictionReactionForce;
            if (Mathf.Abs(jt) < j * staticFrictionAverage)
            {
                frictionReactionForce = jt * tangent;
            }
            else
            {
                frictionReactionForce = -j * tangent * dynamicFrictionAverage;
            }

            Vector3 bodyAForce = (impulseReactionForce - frictionReactionForce) * (1 / bodyA.Mass);
            Vector3 bodyBForce = (impulseReactionForce - frictionReactionForce) * (1 / bodyB.Mass);

            bodyA.SetVelocity(velocityA - bodyAForce);
            bodyB.SetVelocity(velocityB + bodyBForce);
            CorrectPositions(collision);

            if(Mathf.Abs(bodyA.Velocity.magnitude) < 0.1f)
            {
                bodyA.SetVelocity(Vector3.zero);
            }

            if (Mathf.Abs(bodyB.Velocity.magnitude) < 0.1f)
            {
                bodyB.SetVelocity(Vector3.zero);
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
