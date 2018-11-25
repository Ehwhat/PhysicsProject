using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct JLineSegment {

    public Vector3 Start
    {
        get { return _start; }
        set { _start = value; modified = true; }
    }

    public Vector3 End
    {
        get { return _end; }
        set { _end = value; modified = true; }
    }

    public float Length
    {
        get { return GetLength(); }
    }

    public float SqrLength
    {
        get { return GetSqrLength(); }
    }

    private bool modified;
    private Vector3 _start;
    private Vector3 _end;

    private float cachedLength;
    private float cachedLengthSqr;

    public JLineSegment(Vector3 start, Vector3 end)
    {
        modified = true;
        _start = start;
        _end = end;
        cachedLength = 0;
        cachedLengthSqr = 0;
    }

    private float GetLength()
    {
        if (modified)
        {
            cachedLength = Vector3.Magnitude(_start - _end);
        }
        return cachedLength;
    }

    private float GetSqrLength()
    {
        if (modified)
        {
            cachedLengthSqr = Vector3.SqrMagnitude(_start - _end);
        }
        return cachedLengthSqr;
    }

    public bool IntersectWithPlane(Plane plane, out Vector3 hitPoint)
    {
        Vector3 lineVector = End - Start;
        float fullDotLength = Vector3.Dot(plane.normal, lineVector);

        float dot = Vector3.Dot(plane.normal, Start);
        float timeOfIntersection = (plane.distance - dot) / fullDotLength;

        if(timeOfIntersection >= 0 && timeOfIntersection <= 1)
        {
            hitPoint = Start + lineVector * timeOfIntersection;
            return true;
        }
        hitPoint = Vector3.zero;
        return false;
    }

}
