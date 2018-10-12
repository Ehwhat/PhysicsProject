using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class JPhysicsSystem {

    public delegate void OnPhysicsComplete();

    public OnPhysicsComplete _onPhysicsComplete = () => { };
  
    private Thread physicsThread;

    List<JEntity> _physicsEntites = new List<JEntity>();
    List<JRigidbody> _physicsRigidbodies = new List<JRigidbody>();

    public JPhysicsSystem()
    {
        while (true)
        {
            PhysicsStep(Time.fixedDeltaTime);
            PhysicsComplete();
        }
    }

    public int RegisterJEntity()
    {
        _physicsEntites.Add(new JEntity());
        return _physicsEntites.Count - 1;
    }

    public int AddRigidBodyToJEntity(int entityID)
    {
        JEntity entity = _physicsEntites[entityID];
        _physicsRigidbodies.Add(new JRigidbody());
        int id = _physicsRigidbodies.Count - 1;
        entity.rigidbodyId = id;
        return id;
    }

    public JRigidbody GetRigidBody(int entityID)
    {
        return _physicsRigidbodies[entityID];
    }

    void PhysicsStep(object dt)
    {
        float deltaTime = (float)dt;
        lock (_physicsRigidbodies)
        {
            for (int i = 0; i < _physicsRigidbodies.Count; i++)
            {
                _physicsRigidbodies[i].position += _physicsRigidbodies[i].velocity * deltaTime;
            }

        }
        Thread.Sleep(System.TimeSpan.FromMilliseconds(deltaTime));
    }

    void PhysicsComplete()
    {
        _onPhysicsComplete();
    }


}
