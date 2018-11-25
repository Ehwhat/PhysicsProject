using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct JSegment {

    public float Min;
    public float Max;

    public float Length
    {
        get { return Max - Min; }
    }

    public JSegment(float min, float max)
    {
        this.Min = min;
        this.Max = max;
    }

    public static bool IsOverlapping(JSegment a, JSegment b)
    {
        return ((b.Min <= a.Max) && (a.Min <= b.Max));
    }
	
}
