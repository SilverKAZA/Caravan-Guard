using UnityEngine;

public class CatmullRomPath : MonoBehaviour
{
    public Transform[] points;

    public Vector3 GetPoint(float t, int i)
    {
        Vector3 p0 = points[ClampIndex(i - 1)].position;
        Vector3 p1 = points[ClampIndex(i)].position;
        Vector3 p2 = points[ClampIndex(i + 1)].position;
        Vector3 p3 = points[ClampIndex(i + 2)].position;

        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    private int ClampIndex(int i)
    {
        if (i < 0)
            i = 0;

        if (i >= points.Length)
            i = points.Length - 1;

        return i;
    }

    private void OnDrawGizmos()
    {
        if (points == null || points.Length < 2) return;

        Gizmos.color = Color.blue;

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 prev = GetPoint(0, i);

            for (int j = 1; j <= 20; j++)
            {
                float t = j / 20f;
                Vector3 next = GetPoint(t, i);
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }
    }
}
