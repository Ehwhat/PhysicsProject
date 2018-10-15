using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPhysicsManager : MonoBehaviour {

    public int SolverIterations = 3;
    [Range(0,4)]
    public float epsilon = 0;
    private static List<JRigidbody> _bodies = new List<JRigidbody>();
    private List<JCollision> _frameCollisions = new List<JCollision>();

    public static void RegisterBody(JRigidbody body)
    {
        _bodies.Add(body);
        Debug.Log("Registered " + body.name);
    }

    public static void DeregisterBody(JRigidbody body)
    {
        _bodies.Remove(body);
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

            if (reactionForceMagnitude < 0)
            {
                continue;
            }
            

            float j = (-(1.0f+ epsilon) * reactionForceMagnitude) / (bodyA.Mass + bodyB.Mass);
            bodyA.SetVelocity(velocityA - ((j * bodyA.Mass)*normal));
            bodyB.SetVelocity(velocityB + ((j * bodyB.Mass)*normal));
            CorrectPositions(collision);
        }
    }

    private void CorrectPositions(JCollision collision)
    {
        const float k_slop = 0.01f; // Penetration allowance
        const float percent = 0.6f; // Penetration percentage to correct

        JRigidbody bodyA = collision.colliderA.owningBody;
        JRigidbody bodyB = collision.colliderB.owningBody;

        Vector3 correction = (Mathf.Max(collision.collisionDepth - k_slop, 0.0f) / (bodyA.Mass + bodyB.Mass)) * percent * -collision.collisionNormal;
        if(!bodyA._isKinematic)
            bodyA.transform.position -= bodyA.Mass * correction;
        if (!bodyB._isKinematic)
            bodyB.transform.position += bodyB.Mass * correction;
        //bodyA.ApplyForce(bodyA.Mass * correction * 200);
        //bodyB.ApplyForce(-bodyB.Mass * correction * 200);

    }

    private void CheckAllBodiesForCollisions()
    {
        _frameCollisions = new List<JCollision>();
        for (int i = 0; i < _bodies.Count; i++)
        {
            for (int j = 0; j < _bodies.Count; j++)
            {
                if (i == j) continue;

                GetBodyCollisions(_bodies[i], _bodies[j], ref _frameCollisions);
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
                if (collidersA[i].TestCollisionWith(collidersB[j], out collision))
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

}
