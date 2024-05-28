using UnityEngine;

namespace MarsFPSKit
{
    public static class RandomExtensions
    {
        /// <summary>
        /// Either returns -1 or 1
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float RandomPosNeg()
        {
            if (Random.Range(-5, 5) > 0) return 1f;
            else return -1f;
        }

        /// <summary>
        /// Returns a Vector2 with random values between min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Vector2 RandomBetweenVector2(Vector2 min, Vector2 max)
        {
            return new Vector2 { x = Random.Range(min.x, max.x), y = Random.Range(min.y, max.y) };
        }

        /// <summary>
        /// Returns a Vector3 with random values between min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Vector3 RandomBetweenVector3(Vector3 min, Vector3 max)
        {
            return new Vector3 { x = Random.Range(min.x, max.x), y = Random.Range(min.y, max.y), z = Random.Range(min.z, max.z) };
        }
    }
}