using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JColliderBase : MonoBehaviour, IJBounds
{
    public abstract Bounds GetBounds();
    public abstract Bounds GenerateBounds();
}
