
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JCollisionSolver<T1, T2> : JCollisionSolver where T1 : JCollider where T2 : JCollider
{

    protected override bool CheckCollision(JCollider colliderA, JCollider colliderB, out JCollision collision)
    {
        collision = null;
        if(colliderA is T1 && colliderB is T2)
        {
            collision = new JCollision(colliderA, colliderB);
            return CheckCollision(colliderA as T1, colliderB as T2, out collision);
        }else if(colliderB is T1 && colliderA is T2)
        {
            collision = new JCollision(colliderB, colliderA);
            return CheckCollision(colliderB as T1, colliderA as T2, out collision);
        }
        return false;
    }

    protected abstract bool CheckCollision(T1 colliderA, T2 colliderB, out JCollision collision);
}

public abstract class JCollisionSolver {

    protected static Dictionary<CompareablePair<System.Type, System.Type>, JCollisionSolver> solvers = new Dictionary<CompareablePair<System.Type, System.Type>, JCollisionSolver>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnInit()
    {
        RegisterCollisionSolver(new JSphereSphereSolver());
        RegisterCollisionSolver(new JSpherePlaneSolver());
        RegisterCollisionSolver(new JSphereCuboidSolver());
        RegisterCollisionSolver(new JCuboidCuboidSolver());
    }

    protected static void RegisterCollisionSolver<T1,T2>(JCollisionSolver<T1, T2> solver) where T1:JCollider where T2:JCollider
    {
        Debug.Log("Register");
        CompareablePair<System.Type> pair = new CompareablePair<System.Type>(typeof(T1), typeof(T2));
        solvers.Add(pair, solver);
    }

    public static bool SolveCollision(JCollider colliderA, JCollider colliderB, out JCollision collision)
    {
        CompareablePair<System.Type> pair = new CompareablePair<System.Type>(colliderA.GetType(), colliderB.GetType());
        if (solvers.ContainsKey(pair))
        {
            return solvers[pair].CheckCollision(colliderA, colliderB, out collision);
        }
        
        collision = null;
        return false;
    }

    protected virtual bool CheckCollision(JCollider colliderA, JCollider colliderB, out JCollision collision) { collision = null; return false; }

    protected bool CheckCollision(JCollider colliderA, JCollider colliderB)
    {
        JCollision collision = new JCollision();
        return CheckCollision(colliderA, colliderB, out collision);
    }

}
