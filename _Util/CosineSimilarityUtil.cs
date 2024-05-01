using Microsoft.Xna.Framework;
using System;

namespace AchievementLib
{
    /// <summary>
    /// Provides utility functions to calculate and convert cosine similarity as well as angles between 
    /// two <see cref="Vector3"/>s.
    /// </summary>
    public static class CosineSimilarityUtil
    {
        /// <summary>
        /// The factor to convert from radians to degrees.
        /// </summary>
        public const float RAD_TO_DEG = (float)(180.0f / Math.PI);

        /// <summary>
        /// The factor to convert from degrees to radians.
        /// </summary>
        public const float DEG_TO_RAD = (float)(Math.PI / 180.0f);

        /// <summary>
        /// Calculates the cosine similarity of <paramref name="a"/> and <paramref name="b"/>. 
        /// The cosine similarity provides information on how similar the direction of the vectors (or the angle 
        /// between them) is. Ranges between -1 and 1.
        /// </summary>
        /// <remarks>
        /// -1 meaning, that the vectors are pointing in the opposite direction.
        /// 0 meaning, that the vectors are orthogonal to each other.
        /// 1 meaning, that the vectors are pointing in the same direction.
        /// </remarks>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>The cosine similarity of <paramref name="a"/> and <paramref name="b"/>.</returns>
        public static float CosineSimilarity(Vector3 a, Vector3 b)
        {
            return Vector3.Dot(a, b) / (a.Length() * b.Length());
        }

        /// <summary>
        /// Calculates the angle in radians from the given <paramref name="cosineSimilarity"/>.
        /// </summary>
        /// <param name="cosineSimilarity"></param>
        /// <returns>The angle in radians corresponding to the given <paramref name="cosineSimilarity"/>.</returns>
        public static float AngleFromCosineSimilarity(float cosineSimilarity)
        {
            return (float) Math.Acos(cosineSimilarity);
        }

        /// <summary>
        /// Calculates the angle in degrees from the given <paramref name="cosineSimilarity"/>.
        /// </summary>
        /// <param name="cosineSimilarity"></param>
        /// <returns>The angle in degrees corresponding to the given <paramref name="cosineSimilarity"/>.</returns>
        public static float AngleFromCosineSimilarityDegree(float cosineSimilarity)
        {
            return ToDegrees(AngleFromCosineSimilarity(cosineSimilarity));
        }

        /// <summary>
        /// Calculates the angle in radians between the given <see cref="Vector3">Vectors</see>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>The angle in radians between the given <see cref="Vector3">Vectors</see>.</returns>
        public static float AngleBetween(Vector3 a, Vector3 b)
        {
            return AngleFromCosineSimilarity(CosineSimilarity(a, b));
        }

        /// <summary>
        /// Calculates the angle in degree between the given <see cref="Vector3">Vectors</see>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>The angle in degree between the given <see cref="Vector3">Vectors</see>.</returns>
        public static float AngleBetweenDegree(Vector3 a, Vector3 b)
        {
            return ToDegrees(AngleBetween(a, b));
        }

        /// <summary>
        /// Converts the given <paramref name="degrees"/> to radians.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static float ToRadians(float degrees)
        {
            return degrees * DEG_TO_RAD;
        }
        /// <summary>
        /// Converts the given <paramref name="radians"/> to degrees.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static float ToDegrees(float radians)
        {
            return radians * RAD_TO_DEG;
        }
    }
}
