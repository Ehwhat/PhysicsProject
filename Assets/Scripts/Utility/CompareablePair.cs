using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CompareablePair<T> : CompareablePair<T, T> where T : class
{
    public CompareablePair(T a, T b) : base(a, b) {}
}

public class CompareablePair<T1,T2> : System.Object where T1 : class where T2 : class
{

    public T1 a;
    public T2 b;

    public CompareablePair(T1 a, T2 b)
    {
        this.a = a;
        this.b = b;
    }

    public override bool Equals(object obj)
    {
        return obj is CompareablePair<T1, T2> && this == (CompareablePair<T1, T2>)obj;
    }

    public override int GetHashCode()
    {
        return a.GetHashCode() + b.GetHashCode();
    }

    public static bool operator ==(CompareablePair<T1,T2> pairA, CompareablePair<T1, T2> pairB)
    {
        return (pairA.a == pairB.a && pairA.b == pairB.b) || (pairA.a == pairB.b && pairA.b == pairB.a);
    }

    public static bool operator !=(CompareablePair<T1, T2> pairA, CompareablePair<T1, T2> pairB)
    {
        return !(pairA == pairB);
    }

    

}
