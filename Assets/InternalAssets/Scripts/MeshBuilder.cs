using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshBuilder
{
    //public static Mesh BuildPlaneMesh(Vector3 xVector, Vector3 yVector, Vector3 center, Vector2 size)
    //{
    //    xVector = xVector.normalized;
    //    yVector = yVector.normalized;

    //    Mesh newMesh = new Mesh();
    //    List<Vector3> points = new List<Vector3>();
    //    List<Vector3> normals = new List<Vector3>();
    //    List<int> triangles = new List<int>();

    //    Vector3 basePoint = center - xVector * size.x / 2 - yVector * size.y / 2;

    //    for (int x = 0; x < pointsPerAxis; ++x)
    //        for (int y = 0; y < pointsPerAxis; ++y)
    //        {
    //            Vector3 newPoint = basePoint;

    //            newPoint += x * size.x / (pointsPerAxis - 1) * xVector;
    //            newPoint += y * size.y / (pointsPerAxis - 1) * yVector;
    //            Vector3 spherePoint = newPoint.normalized * size.x/2;

    //            Vector3 realPoint = spherePoint + spherePoint * 0.3f * Noise.GeneratePoint(0, noiseSpan, spherePoint + Vector3.one * 5000);

    //            points.Add(realPoint);
    //            //points.Add(realPoint);

    //            normals.Add(newPoint.normalized);

    //            if (x == 0 || y == 0) // We should have at least 3 points to make surface
    //                continue;

    //            triangles.Add(pointsPerAxis * (y - 1) + x - 1);
    //            triangles.Add(pointsPerAxis * y + x);
    //            triangles.Add(pointsPerAxis * y + x - 1);


    //            triangles.Add(pointsPerAxis * (y - 1) + x - 1);
    //            triangles.Add(pointsPerAxis * (y - 1) + x);
    //            triangles.Add(pointsPerAxis * y + x);

    //        }

    //    newMesh.vertices = points.ToArray();
    //    newMesh.triangles = triangles.ToArray();
    //    newMesh.normals = normals.ToArray();

    //    return newMesh;
    //}
    const int pointsPerAxis = 32;
    const int noiseOctaves = 4;
    const float noiseSpan = 35;

    public static Mesh BuildPlaneMesh(Vector3 xVector, Vector3 yVector, Vector3 center, Vector2 size) // With dublicating points
    {
        xVector = xVector.normalized;
        yVector = yVector.normalized;
        int squarePerAxis = pointsPerAxis - 1;

        Mesh newMesh = new Mesh();
        List<Vector3> points = new List<Vector3>(squarePerAxis * squarePerAxis * 4);
        List<Vector3> normals = new List<Vector3>(squarePerAxis * squarePerAxis * 4);
        List<int> triangles = new List<int>(squarePerAxis * squarePerAxis * 2 * 3);

        Vector3 basePoint = center - xVector * size.x / 2 - yVector * size.y / 2;


        for (int x = 0; x < squarePerAxis; ++x)
            for (int y = 0; y < squarePerAxis; ++y)
            {
                Vector3 newPointBase = basePoint;

                newPointBase += x * size.x / squarePerAxis * xVector;
                newPointBase += y * size.y / squarePerAxis * yVector;

                Vector3[] squarePoints = new Vector3[4]
                {
                    newPointBase,
                    newPointBase + size.x / squarePerAxis * xVector,
                    newPointBase + size.y / squarePerAxis * yVector,
                    newPointBase + size.x / squarePerAxis * xVector + size.y / squarePerAxis * yVector
                };

                Vector3[] spherePoints = new Vector3[4];
                Vector3[] realPoints = new Vector3[4];

                int[] vertexIndexes = new int[4];

                for (int i = 0; i < 4; ++i)
                {
                    spherePoints[i] = squarePoints[i].normalized * size.x / 2;

                    Vector3 finalNoisePoint = spherePoints[i];

                    for (int oct = 0; oct < noiseOctaves; ++oct)
                    {
                        float noiseValue = (0.5f / (oct + 1)) * 0.3f * Noise.GeneratePoint(0, noiseSpan / (oct + 1), spherePoints[i] + Vector3.one * 5000);
                        finalNoisePoint += spherePoints[i] * noiseValue;
                    }



                    realPoints[i] = finalNoisePoint;
                    points.Add(realPoints[i]);
                    vertexIndexes[i] = points.Count - 1;
                    normals.Add(realPoints[i].normalized);
                }

                triangles.Add(vertexIndexes[0]);
                triangles.Add(vertexIndexes[2]);
                triangles.Add(vertexIndexes[1]);

                triangles.Add(vertexIndexes[1]);
                triangles.Add(vertexIndexes[2]);
                triangles.Add(vertexIndexes[3]);
            }

        newMesh.vertices = points.ToArray();
        newMesh.triangles = triangles.ToArray();
        newMesh.normals = normals.ToArray();

        return newMesh;
    }
}
