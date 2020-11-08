using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorHelper {
    public static Vector3 RandomVector3() => RandomVector3(0, 1);
    public static Vector3 RandomVector3(float min, float max) {
        return new Vector3(
            Random.Range(min, max),
            Random.Range(min, max),
            Random.Range(min, max));
    }

    public static bool WithinRange(Vector3 u, Vector3 v, float range) {
        return (u - v).sqrMagnitude <= range * range;
    }
}
