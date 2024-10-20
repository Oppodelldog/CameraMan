using UnityEngine;

namespace CameraMan
{
    public abstract class Smoothing
    {
        private static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            var t2 = t * t;
            var t3 = t2 * t;

            var a = 2f * p1;
            var b = p2 - p0;
            var c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            var d = -p0 + 3f * p1 - 3f * p2 + p3;

            return 0.5f * (a + b * t + c * t2 + d * t3);
        }

        internal static void SmoothTrack(Transform container, int steps = 1)
        {
            for (var step = 0; step < steps; step++)
            {
                for (var i = 0; i < container.childCount; i++)
                {
                    if (i == 0 || i == container.childCount - 1)
                        continue;

                    var newPos = CatmullRom(
                        container.GetChild(i).position,
                        container.GetChild(Mathf.Max(i - 1, 0)).position,
                        container.GetChild(Mathf.Min(i + 1, container.childCount - 1)).position,
                        container.GetChild(Mathf.Min(i + 2, container.childCount - 1)).position,
                        0.5f);

                    container.GetChild(i).position = newPos;
                }
            }
        }
    }
}