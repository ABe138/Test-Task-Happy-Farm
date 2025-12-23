using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ObjectsScanner
{
    public static List<T> FindObjectsInRange<T>(Vector3 origin, float radius, LayerMask layerMask)
    {
        var cols = Physics.OverlapSphere(origin, radius, layerMask);
        var result = new List<T>(cols.Length);
        foreach (var c in cols)
        {
            var t = c.GetComponent<T>();
            if (t != null) result.Add(t);
        }
        return result;
    }

    public static List<T> FindObjectsInArc<T>(Vector3 origin, Vector3 centerDir, float radius, float halfAngleDeg, LayerMask layerMask)
    {
        var cols = Physics.OverlapSphere(origin, radius, layerMask);
        var result = new List<T>();
        var halfRad = Mathf.Deg2Rad * halfAngleDeg;
        var cosThreshold = Mathf.Cos(halfRad);
        var dirNormalized = centerDir.normalized;
        foreach (var c in cols)
        {
            Vector3 to = (c.transform.position - origin);
            to.y = 0f;
            if (to.sqrMagnitude < 0.0001f || Vector3.Dot(dirNormalized, to.normalized) >= cosThreshold) 
            {
                var t = c.GetComponent<T>();
                if (t != null) result.Add(t);
            }
        }
        return result;
    }
}
