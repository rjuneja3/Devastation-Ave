using UnityEngine;

namespace Assets.Scripts.Helpers {

    /**
     * @author Brenton Hauth
     * @date 10/20/20
     * <summary>
     * General functions to help with vectors
     * </summary>
     */
    public static class VectorHelper {
        /**
         * @author Brenton Hauth
         * @date 10/20/20
         * <summary>
         * Produces a random Vector3 with values between 0 and 1
         * </summary>
         */
        public static Vector3 RandomVector3() => RandomVector3(0, 1);

        /**
         * @author Brenton Hauth
         * @date 10/20/20
         * <summary>
         * Produces a random Vector3 with values between min and max
         * </summary>
         * <param name="min">The minimum value that an element can be</param>
         * <param name="max">The maximum value that an element can be</param>
         */
        public static Vector3 RandomVector3(float min, float max) {
            return new Vector3(
                Random.Range(min, max),
                Random.Range(min, max),
                Random.Range(min, max));
        }

        /**
         * @author Brenton Hauth
         * @date 11/11/20
         * <summary>
         * Produces a random Vector2 with values between 0 and 1
         * </summary>
         */
        public static Vector2 RandomVector2() => RandomVector2(0, 1);

        /**
         * @author Brenton Hauth
         * @date 11/11/20
         * <summary>
         * Produces a random Vector2 with values between min and max
         * </summary>
         * <param name="min">The minimum value that an element can be</param>
         * <param name="max">The maximum value that an element can be</param>
         */
        public static Vector2 RandomVector2(float min, float max) {
            return new Vector3(
                Random.Range(min, max),
                Random.Range(min, max));
        }

        /**
         * @author Brenton Hauth
         * @date 10/20/20
         * <summary>
         * Checks if 2 points are within range of eachother
         * </summary>
         * <param name="u">The first vector to check</param>
         * <param name="v">The second vector to check</param>
         * <param name="range">The range to check</param>
         */
        public static bool WithinRange(Vector3 u, Vector3 v, float range) {
            return Vector3.SqrMagnitude(u - v) <= range * range;
        }
        
        /**
         * @author Brenton Hauth
         * @date 12/01/20
         * <summary>
         * Returns a vector with it's Y value as 0.
         * </summary>
         * <param name="v">Vector to replace y value with 0</param>
         */
        public static Vector3 X0Z(this Vector3 v) {
            // Quicker than 'new Vector3(v.x, 0, v.z)'
            v.y = 0;
            return v;
        }
    }
}
