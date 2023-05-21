using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static SphereChunk;

public static class MeshBuilder
{
    // BuildPlaneMesh halfVertex way 
    //public static Mesh BuildPlaneMesh(SphereChunk.ChunkParams chunkParams)
    //{
    //    Vector3 xVector = chunkParams.xVector;
    //    Vector3 yVector = chunkParams.yVector;
    //    Vector3 center = chunkParams.chunkCenter;
    //    Vector2 size = chunkParams.chunkSize;
    //    float radius = chunkParams.radius;


    //    xVector = xVector.normalized;
    //    yVector = yVector.normalized;

    //    Mesh newMesh = new Mesh();
    //    List<Vector3> points = new List<Vector3>(pointsPerAxis * pointsPerAxis);
    //    List<Vector3> normals = new List<Vector3>(pointsPerAxis * pointsPerAxis);
    //    List<int> triangles = new List<int>();
    //    List<Vector2> uv = new List<Vector2>(pointsPerAxis * pointsPerAxis);

    //    Vector3 basePoint = center - xVector * size.x / 2 - yVector * size.y / 2;

    //    for (int x = 0; x < pointsPerAxis; ++x)
    //        for (int y = 0; y < pointsPerAxis; ++y)
    //        {
    //            Vector3 newPoint = basePoint;

    //            newPoint += x * size.x / (pointsPerAxis - 1) * xVector;
    //            newPoint += y * size.y / (pointsPerAxis - 1) * yVector;
    //            Vector3 spherePoint = newPoint.normalized * radius;

    //            Vector3 realPoint = spherePoint + spherePoint * 0.3f * Noise.GeneratePoint(0, noiseSpan, spherePoint + Vector3.one * 5000);

    //            points.Add(realPoint);
    //            normals.Add(newPoint.normalized);
    //            uv.Add(new Vector2((float)x / pointsPerAxis, (float)y / pointsPerAxis));

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
    //    newMesh.uv = uv.ToArray();

    //    return newMesh;
    //}
    const int pointsPerAxis = 32;
    const int noiseOctaves = 4;
    const int noiseSpan = 35;

    // BuildPlaneMesh (Unoptimised algorithm) With Vertex Dublication
    //public static Mesh BuildPlaneMesh(SphereChunk.ChunkParams chunkParams) // With dublicating points
    //{
    //    Vector3 xVector = chunkParams.xVector;
    //    Vector3 yVector = chunkParams.yVector;
    //    Vector3 center = chunkParams.chunkCenter;
    //    Vector2 size = chunkParams.chunkSize;
    //    float radius = chunkParams.radius;


    //    xVector = xVector.normalized;
    //    yVector = yVector.normalized;
    //    int squarePerAxis = pointsPerAxis - 1;

    //    Mesh newMesh = new Mesh();
    //    List<Vector3> points = new List<Vector3>(squarePerAxis * squarePerAxis * 4);
    //    List<Vector3> normals = new List<Vector3>(squarePerAxis * squarePerAxis * 4);
    //    List<int> triangles = new List<int>(squarePerAxis * squarePerAxis * 2 * 3);

    //    Vector3 basePoint = center - xVector * size.x / 2 - yVector * size.y / 2;


    //    for (int x = 0; x < squarePerAxis; ++x)
    //        for (int y = 0; y < squarePerAxis; ++y)
    //        {
    //            Vector3 newPointBase = basePoint;

    //            newPointBase += x * size.x / squarePerAxis * xVector;
    //            newPointBase += y * size.y / squarePerAxis * yVector;

    //            Vector3[] squarePoints = new Vector3[4]
    //            {
    //                newPointBase,
    //                newPointBase + size.x / squarePerAxis * xVector,
    //                newPointBase + size.y / squarePerAxis * yVector,
    //                newPointBase + size.x / squarePerAxis * xVector + size.y / squarePerAxis * yVector
    //            };

    //            Vector3[] spherePoints = new Vector3[4];
    //            Vector3[] realPoints = new Vector3[4];

    //            int[] vertexIndexes = new int[4];

    //            for (int i = 0; i < 4; ++i)
    //            {
    //                spherePoints[i] = squarePoints[i].normalized * radius;

    //                Vector3 finalNoisePoint = spherePoints[i];

    //                for (int oct = 0; oct < noiseOctaves; ++oct)
    //                {
    //                    float baseNoiseValue = Noise.GeneratePoint(0, noiseSpan / (oct + 1), spherePoints[i] + Vector3.one * 5000);
    //                    float modifiedNoiseValue = (0.5f / (oct + 1)) * 0.3f * baseNoiseValue;
    //                    finalNoisePoint += spherePoints[i] * modifiedNoiseValue;
    //                }



    //                realPoints[i] = finalNoisePoint;
    //                points.Add(realPoints[i]);
    //                vertexIndexes[i] = points.Count - 1;
    //                normals.Add(realPoints[i].normalized);
    //            }

    //            triangles.Add(vertexIndexes[0]);
    //            triangles.Add(vertexIndexes[2]);
    //            triangles.Add(vertexIndexes[1]);

    //            triangles.Add(vertexIndexes[1]);
    //            triangles.Add(vertexIndexes[2]);
    //            triangles.Add(vertexIndexes[3]);
    //        }

    //    newMesh.vertices = points.ToArray();
    //    newMesh.triangles = triangles.ToArray();
    //    newMesh.normals = normals.ToArray();

    //    return newMesh;
    //}
    public static Mesh BuildPlaneMesh(SphereChunk.ChunkParams chunkParams) // With dublicating points
    {
        Vector3 xVector = chunkParams.xVector;
        Vector3 yVector = chunkParams.yVector;
        Vector3 center = chunkParams.chunkCenter;
        Vector2 size = chunkParams.chunkSize;
        float radius = chunkParams.radius;


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
                    spherePoints[i] = squarePoints[i].normalized * radius;

                    Vector3 finalNoisePoint = spherePoints[i];

                    for (int oct = 0; oct < noiseOctaves; ++oct)
                    {
                        float baseNoiseValue = Noise.GeneratePoint(0, noiseSpan / (oct + 1), spherePoints[i] + Vector3.one * 5000);
                        float modifiedNoiseValue = (0.5f / (oct + 1)) * 0.3f * baseNoiseValue;
                        finalNoisePoint += spherePoints[i] * modifiedNoiseValue;
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
    //public static Mesh BuildPlaneMesh(Vector3 xVector, Vector3 yVector, Vector3 center, Vector2 size, float radius) // With dublicating points
    //{
    //    xVector = xVector.normalized;
    //    yVector = yVector.normalized;
    //    int squarePerAxis = pointsPerAxis - 1;

    //    Mesh newMesh = new Mesh();
    //    List<Vector3> points = new List<Vector3>(squarePerAxis * squarePerAxis * 4);
    //    List<Vector3> normals = new List<Vector3>(squarePerAxis * squarePerAxis * 4);
    //    List<int> triangles = new List<int>(squarePerAxis * squarePerAxis * 2 * 3);

    //    Vector3 basePoint = center - xVector * size.x / 2 - yVector * size.y / 2;


    //    for (int x = 0; x < squarePerAxis; ++x)
    //        for (int y = 0; y < squarePerAxis; ++y)
    //        {
    //            Vector3 newPointBase = basePoint;

    //            newPointBase += x * size.x / squarePerAxis * xVector;
    //            newPointBase += y * size.y / squarePerAxis * yVector;

    //            Vector3[] squarePoints = new Vector3[4]
    //            {
    //                newPointBase,
    //                newPointBase + size.x / squarePerAxis * xVector,
    //                newPointBase + size.y / squarePerAxis * yVector,
    //                newPointBase + size.x / squarePerAxis * xVector + size.y / squarePerAxis * yVector
    //            };

    //            Vector3[] spherePoints = new Vector3[4];
    //            Vector3[] realPoints = new Vector3[4];

    //            int[] vertexIndexes = new int[4];

    //            for (int i = 0; i < 4; ++i)
    //            {
    //                spherePoints[i] = squarePoints[i].normalized * radius;

    //                Vector3 finalNoisePoint = spherePoints[i];

    //                for (int oct = 0; oct < noiseOctaves; ++oct)
    //                {
    //                    float baseNoiseValue = Noise.GeneratePoint(0, noiseSpan / (oct + 1), spherePoints[i] + Vector3.one * 5000);
    //                    float modifiedNoiseValue = (0.5f / (oct + 1)) /** 0.3f*/ * baseNoiseValue;
    //                    finalNoisePoint += spherePoints[i] * modifiedNoiseValue;
    //                }



    //                realPoints[i] = finalNoisePoint;
    //                points.Add(realPoints[i]);
    //                vertexIndexes[i] = points.Count - 1;
    //                normals.Add(realPoints[i].normalized);
    //            }

    //            triangles.Add(vertexIndexes[0]);
    //            triangles.Add(vertexIndexes[2]);
    //            triangles.Add(vertexIndexes[1]);

    //            triangles.Add(vertexIndexes[1]);
    //            triangles.Add(vertexIndexes[2]);
    //            triangles.Add(vertexIndexes[3]);
    //        }

    //    newMesh.vertices = points.ToArray();
    //    newMesh.triangles = triangles.ToArray();
    //    newMesh.normals = normals.ToArray();

    //    return newMesh;
    //}

    //public static void BuildPlaneMesh(Mesh mesh, SphereChunk.ChunkParams chunkParams) // With dublicating points
    //{
    //    Vector3 xVector = chunkParams.xVector;
    //    Vector3 yVector = chunkParams.yVector;
    //    Vector3 center = chunkParams.chunkCenter;
    //    Vector2 size = chunkParams.chunkSize;
    //    float radius = chunkParams.radius;

    //    xVector = xVector.normalized;
    //    yVector = yVector.normalized;
    //    int squarePerAxis = pointsPerAxis - 1;

    //    List<Vector3> normals = new List<Vector3>(squarePerAxis * squarePerAxis * 4);

    //    Vector3 basePoint = center - xVector * size.x / 2 - yVector * size.y / 2;

    //    int vertexIndex = 0;
    //    for (int x = 0; x < squarePerAxis; ++x)
    //        for (int y = 0; y < squarePerAxis; ++y, ++vertexIndex)
    //        {
    //            Vector3 newPointBase = basePoint;

    //            newPointBase += x * size.x / squarePerAxis * xVector;
    //            newPointBase += y * size.y / squarePerAxis * yVector;

    //            Vector3[] squarePoints = new Vector3[4]
    //            {
    //                newPointBase,
    //                newPointBase + size.x / squarePerAxis * xVector,
    //                newPointBase + size.y / squarePerAxis * yVector,
    //                newPointBase + size.x / squarePerAxis * xVector + size.y / squarePerAxis * yVector
    //            };

    //            Vector3[] spherePoints = new Vector3[4];
    //            Vector3[] realPoints = new Vector3[4];

    //            int[] vertexIndexes = new int[4];

    //            for (int i = 0; i < 4; ++i)
    //            {
    //                spherePoints[i] = squarePoints[i].normalized * radius;

    //                Vector3 finalNoisePoint = spherePoints[i];

    //                for (int oct = 0; oct < noiseOctaves; ++oct)
    //                {
    //                    float noiseValue = Noise.GeneratePoint(0, noiseSpan / (oct + 1), spherePoints[i] + Vector3.one * 5000);
    //                    float modifiedNoiseValue = (0.5f / (oct + 1)) * 0.3f * noiseValue;
    //                    finalNoisePoint += spherePoints[i] * (noiseValue - 0.5f);
    //                }



    //                realPoints[i] = finalNoisePoint;
    //                mesh.vertices[vertexIndex] = realPoints[i];
    //                normals.Add(realPoints[i].normalized);
    //            }
    //        }

    //    mesh.normals = normals.ToArray();
    //    mesh.RecalculateNormals();
    //    mesh.RecalculateTangents();
    //}
}
