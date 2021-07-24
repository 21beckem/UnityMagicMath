using UnityEngine;

public static class Poly
{
    /// <summary>
    /// Determines if a vector2 is inside of an polygon whos points are in a vector2 array
    /// </summary>
    /// <param name="polyPoints">array of vector2s that make up a polygon</param>
    /// <param name="p">coordinate that could be inside or outside the polygon</param>
    /// <returns></returns>
    public static bool ContainsPoint(Vector2[] polyPoints, Vector2 p)
    {
        var j = polyPoints.Length - 1;
        var inside = false;
        for (int i = 0; i < polyPoints.Length; j = i++)
        {
            var pi = polyPoints[i];
            var pj = polyPoints[j];
            if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
                (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                inside = !inside;
        }
        return inside;
    }
    public static Vector2[] circle(float radius, int resolution)
    {
        Vector2[] verticies = new Vector2[resolution];
        float x;
        float z;
        for (int i = 0; i < resolution; i++)
        {
            x = radius * Mathf.Sin((2 * Mathf.PI * i) / resolution);
            z = radius * Mathf.Cos((2 * Mathf.PI * i) / resolution);
            verticies[i] = new Vector2(x, z);
        }
        return verticies;
    }
    public static Vector2[] squareAtAngle(float sideLength, float angleDeg)
    {
        float rads = ((angleDeg + 45) * Mathf.PI) / 180;
        float radius = sideLength / 1.414f;
        Vector2[] verticies = new Vector2[4];
        float x;
        float z;
        for (int i = 0; i < 4; i++)
        {
            float math = (2 * Mathf.PI * i) / 4 + rads;
            x = radius * Mathf.Sin(math);
            z = radius * Mathf.Cos(math);
            verticies[i] = new Vector2(x, z);
        }
        return verticies;
    }
    /// <summary>
    /// makes vertices for sort of rounded square
    /// </summary>
    /// <param name="radius">radius of the circle</param>
    /// <param name="mainRotation">main rotation of the square</param>
    /// <param name="chamfer">angle of how much to be chamfered</param>
    /// <returns></returns>
    public static Vector2[] roundedSquareWithAngle(float radius, float mainRotation, float chamfer)
    {
        if (chamfer == 0)
        {
            return squareAtAngle(radius * 1.414f, mainRotation);
        }
        float angle1 = mainRotation + 45 - (chamfer / 2);
        float angle2 = mainRotation + 45 + (chamfer / 2);
        //float radius = sideLength / 1.414f;
        Vector2[] verticies = new Vector2[8];
        float x;
        float z;
        for (int i = 0, n = 0; n < 4; n++)
        {
            x = radius * Mathf.Sin((((90 * n) + angle1) * Mathf.PI) / 180);
            z = radius * Mathf.Cos((((90 * n) + angle1) * Mathf.PI) / 180);
            verticies[i] = new Vector2(x, z);
            x = radius * Mathf.Sin((((90 * n) + angle2) * Mathf.PI) / 180);
            z = radius * Mathf.Cos((((90 * n) + angle2) * Mathf.PI) / 180);
            verticies[i + 1] = new Vector2(x, z);
            i += 2;
        }
        return verticies;
    }

    public static Vector2[] lumpyCircle(float radius, int resolution, float perlinScale, float perlinInfluence, float offX, float offY)
    {
        Vector2[] verticies = new Vector2[resolution];
        float x;
        float z;
        for (int i = 0; i < resolution; i++)
        {
            float r = Mathf.PerlinNoise(Mathf.Sin((2 * Mathf.PI * i) / resolution * perlinScale) + 1 + offX, Mathf.Cos((2 * Mathf.PI * i) / resolution * perlinScale) + 1 + offY) * perlinInfluence;
            r += radius;
            x = r * Mathf.Sin((2 * Mathf.PI * i) / resolution);
            z = r * Mathf.Cos((2 * Mathf.PI * i) / resolution);
            verticies[i] = new Vector2(x, z);
        }
        return verticies;
    }

    public static Vector2 projectThroughPoint(Vector2 center, Vector2 point, float d)
    {
        float s = (point.y - center.y) / (point.x - center.x);
        float x2 = (d / (Mathf.Sqrt(1 + (s*s)))) + point.x;

        float xDis = x2 - point.x;
        float y2 = Mathf.Sqrt((d * d) - (xDis * xDis)) + point.y;

        return new Vector2(x2, y2);
    }

    public static Vector2[] lumpyPoly(Vector2[] points, Vector2 center, float perlinInfluence1, float scale1, float perlinInfluence2, float scale2, bool random = true)
    {
        Vector2 off1;
        Vector2 off2;
        if (random)
        {
            off1 = new Vector2(Random.Range(1, 999999), Random.Range(1, 999999));
            off2 = new Vector2(Random.Range(1, 999999), Random.Range(1, 999999));
        }
        else
        {
            off1 = new Vector2(1, 1);
            off2 = new Vector2(1, 1);
        }
        Vector2[] output = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            float per = Mathf.PerlinNoise((points[i].x + off1.x) / scale1, (points[i].y + off1.y) / scale1) * perlinInfluence1;
            per += Mathf.PerlinNoise((points[i].x + off2.x) / scale2, (points[i].y + off2.y) / scale2) * perlinInfluence2;
            output[i] = projectThroughPoint(center, points[i], per);
        }
        return output;
    }

    public static Vector2[] offsetPoly(Vector2[] points, float offset)
    {
        Vector2[] output = new Vector2[points.Length];
        for (int ii = 0; ii < points.Length; ii++)
        {
            int i = (ii - 1 == -1) ? points.Length - 1 : ii - 1;
            int iii = (ii + 1 == points.Length) ? 0 : ii + 1;
            output[ii] = meanAngleProjection(points[i], points[ii], points[iii], offset);
        }
        return output;
    }

    /// <summary>
    /// Returns angle about 3 points
    /// </summary>
    /// <param name="point1">Frist Point</param>
    /// <param name="center">Center Point</param>
    /// <param name="point2">Second Point</param>
    /// <returns>float between 0 and 360</returns>
    public static float findAngle(Vector2 point1, Vector2 center, Vector2 point2)
    {
        //Vector2 v2 = (point2) - (point1);
        //return -Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
        float math = -Vector2.SignedAngle(point1 - center, point2 - center);
        if (math < 0)
            math += 360;
        return math;
    }
    public static Vector2 meanAngleProjection(Vector2 point1, Vector2 center, Vector2 point2, float distance)
    {
        float math1 = findAngle(center + Vector2.up, center, point1);
        float math2 = findAngle(point1, center, point2);
        float angle = math1 + (math2 / 2);
        Vector2 result;
        result.x = distance * Mathf.Sin(angle * Mathf.Deg2Rad);
        result.y = distance * Mathf.Cos(angle * Mathf.Deg2Rad);
        result += center;
        return result;
    }
}
