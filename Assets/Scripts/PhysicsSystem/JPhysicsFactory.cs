using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JPhysicsFactory {

    public static JPhysicsSystem system;

    [RuntimeInitializeOnLoadMethod]
    public static void InitaliseScenes()
    {
        Debug.Log("init");
        system = new JPhysicsSystem();

        JRigidbodyComponent[] rigidbodyComponents = Object.FindObjectsOfType<JRigidbodyComponent>();
        foreach (JRigidbodyComponent component in rigidbodyComponents)
        {
            RegisterRigidBody(component);
        }
    }

    private static int RegisterRigidBody(JRigidbodyComponent component)
    {
        int entityId = system.RegisterJEntity();
        int rigidbodyId = system.AddRigidBodyToJEntity(entityId);
        JPhysicsSystem.OnPhysicsComplete callback = () => { };
        callback = () =>
        {
            JRigidbody jrb = system.GetRigidBody(entityId);
            if (jrb == null)
            {
                system._onPhysicsComplete -= callback;
            }
            else
            {
                component.transform.position = jrb.position;
            }
        };

        system._onPhysicsComplete += callback;
        return entityId;
    }


	
}
