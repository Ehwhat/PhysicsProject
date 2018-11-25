using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JRigidbody : MonoBehaviour, IJBounds {

    /*  So this is gonna be the Rigidbody class.
     *  Lets use the Transform component all gameobjects have to model this simply
     */

    private const float MASS_MIN = 0.0001f;

    public bool _isKinematic = false;
    public bool _isAffectedByGravity = true;

    public float Mass
    {
        get { if (_isKinematic) return 0; return _mass; }
        set { _mass = Mathf.Max(value, MASS_MIN); }
    }

    public Vector3 Velocity { get; private set; }
    public Vector3 AngularVelocity { get; set; }

    public Vector3 lastForce;
    public Vector3 localCenterOfMass;

    [SerializeField]
    private float _mass = 1;

    [SerializeField]
    public float StaticFriction = 0.3f;

    [SerializeField]
    public float DynamicFriction = 0.1f;

    private Vector3 _currentForce;
    private Vector3 _currentTorque;

    public JCollider[] _colliders;
    private int _childCount;
    private Bounds _localBounds;

    private void OnEnable()
    {
        JPhysicsManager.RegisterBody(this);
    }

    private void Start()
    {
        RegisterColliders();
    }

    private void OnDisable()
    {
        JPhysicsManager.DeregisterBody(this);
    }

    private void RegisterColliders()
    {
        _colliders = GetComponentsInChildren<JCollider>();
        for (int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].owningBody = this;
        }
        _childCount = transform.hierarchyCount;
        
        GenerateBounds();
    }

    private void CheckIfChangedColliders()
    {
        if (transform.hierarchyCount != _childCount)
        {
            RegisterColliders();
        }
    }

    public void ApplyForce(Vector3 force)
    {
        if (_isKinematic)
            return;
        _currentForce += force;
    }

    public void ApplyImpulse(Vector3 impulse)
    {
        if (_isKinematic)
            return;
        Velocity += impulse * _mass;
    }

    public void ApplyAngularImpulse(Vector3 point, Vector3 impulse)
    {
        if (_isKinematic)
            return;
        Vector3 centerOfMass = GetCenterOfMass();
        Vector3 torque = Vector3.Cross(point - centerOfMass, impulse);
        Vector3 angularAcceleration = _colliders[0].GetInverseTensor(Mass) * torque;
        AngularVelocity += angularAcceleration;
    }

    public float GetInvMass()
    {
        if(Mass == 0)
        {
            return 0;
        }
        return 1 / Mass;
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        if (!_isKinematic)
        {
            Velocity = newVelocity;
        }
    }

    public void Simulate(float dt)
    {
        GenerateBounds();
        if (!_isKinematic)
        {
            if (_isAffectedByGravity)
            {
                ApplyForce(Physics.gravity);
            }

            Vector3 _acceleration = _currentForce / _mass;
            Velocity += _acceleration * dt;
            transform.position += Velocity * dt;

            Vector3 angularAcceleration = _colliders[0].GetInverseTensor(Mass) * _currentTorque;
            AngularVelocity += angularAcceleration * dt;
            transform.Rotate(AngularVelocity * dt);


            lastForce = _currentForce;
            _currentForce = Vector3.zero;
        }
    }

    public JCollider[] GetColliders()
    {
        return _colliders;
    }

    public Vector3 GetCenterOfMass()
    {
        if(_colliders.Length < 1)
        {
            return transform.position;
        }
        Vector3 center = Vector3.zero;
        for (int i = 0; i < _colliders.Length; i++)
        {
            center += _colliders[i].transform.position;
        }
        return center /= _colliders.Length;
    }

    private void GenerateBounds()
    {
        _localBounds = new Bounds(transform.position, Vector3.zero);
        Vector3 center = Vector3.zero;
        for (int i = 0; i < _colliders.Length; i++)
        {
            Bounds colliderBounds = _colliders[i].GenerateBounds();
            colliderBounds.center = _colliders[i].transform.position;
            center += _colliders[i].transform.position;
            _localBounds.Encapsulate(colliderBounds);
        }
        center /= _colliders.Length;
        localCenterOfMass = transform.InverseTransformPoint(center);
    }

    public Bounds GetBounds()
    {
        _localBounds.center = transform.position + localCenterOfMass;
        return _localBounds;
    }

    private void OnDrawGizmos()
    {
        Bounds bounds = GetBounds();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
