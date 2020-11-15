using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class VectorHelper {
        public static Vector3 RandomVector3() => RandomVector3(0, 1);
        public static Vector3 RandomVector3(float min, float max) {
            return new Vector3(
                Random.Range(min, max),
                Random.Range(min, max),
                Random.Range(min, max));
        }

        public static Vector2 RandomVector2() => RandomVector2(0, 1);
        public static Vector2 RandomVector2(float min, float max) {
            return new Vector3(
                Random.Range(min, max),
                Random.Range(min, max));
        }

        public static bool WithinRange(Vector3 u, Vector3 v, float range) {
            return Vector3.SqrMagnitude(u - v) <= range * range;
        }

        public static bool WithinRange2(Vector3 u, Vector3 v, float range) {
            return Vector3.Distance(u, v) <= range;
        }
    }
}
