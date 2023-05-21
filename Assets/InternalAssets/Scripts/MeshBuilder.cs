using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Entities.UniversalDelegates;
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

    //BuildPlaneMesh(Unoptimised algorithm) With Vertex Dublication
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



    public static Mesh BuildPlaneMesh(SphereChunk.ChunkParams chunkParams) // With dublicating points // Optimized
    {
        Vector3 xVector = chunkParams.xVector.normalized;
        Vector3 yVector = chunkParams.yVector.normalized;
        Vector3 center = chunkParams.chunkCenter;
        Vector2 size = chunkParams.chunkSize;
        float radius = chunkParams.radius;

        Mesh newMesh = new Mesh();

        int squarePerAxis = pointsPerAxis - 1;

        int pointsOnPlane = pointsPerAxis * pointsPerAxis;
        int squaresOnPlane = squarePerAxis * squarePerAxis;

        int finalVertexCount = squaresOnPlane * 4;

        Vector3[] finalPoints = new Vector3[finalVertexCount];
        Vector3[] normals = new Vector3[finalVertexCount];

        int[] triangles = new int[squaresOnPlane * 6]; // 6 = 2 triangles per square * 3 vert per triangle

        Vector3 basePoint = center - xVector * size.x / 2 - yVector * size.y / 2;

        Vector3[] points = new Vector3[pointsOnPlane];
        int pointIndex = 0;

        Vector3 offsetX = size.x / (pointsPerAxis - 1) * xVector;
        Vector3 offsetY = size.y / (pointsPerAxis - 1) * yVector;

        for (int x = 0; x < pointsPerAxis; ++x)
            for (int y = 0; y < pointsPerAxis; ++y, ++pointIndex)
            {
                points[pointIndex] = basePoint;
                points[pointIndex] += x * offsetX;
                points[pointIndex] += y * offsetY;
                points[pointIndex] = points[pointIndex].normalized * radius;
            } // Every point on ideal sphere

        float[][] noiseResults = new float[noiseOctaves][];


        float finalKoef = 0.3f * 0.5f;
        for (int oct = 0; oct < noiseOctaves; ++oct)
        {
            noiseResults[oct] = Noise.GeneratePoints(0, noiseSpan / (oct + 1), points);

            for (int i = 0; i < points.Length; ++i)
            {
                float modifiedNoiseValue = finalKoef / (oct + 1) * noiseResults[oct][i];
                points[i] += points[i] * modifiedNoiseValue;
            }
        }
        // Next step is just compose squares from those points

        Vector3[,] points2D = new Vector3[pointsPerAxis, pointsPerAxis];

        for (int i = 0; i < pointsPerAxis; ++i)
            for (int j = 0; j < pointsPerAxis; ++j)
            {
                points2D[i, j] = points[i * pointsPerAxis + j];
            }

        int finalPointsIter = 0;
        int trianglesIter = 0;
        int max = 0;
        for (int i = 1; i < pointsPerAxis; ++i)
            for (int j = 1; j < pointsPerAxis; ++j)
            {
                normals[finalPointsIter] = points2D[i - 1, j - 1];
                finalPoints[finalPointsIter++] = points2D[i - 1, j - 1];

                normals[finalPointsIter] = points2D[i, j - 1];
                finalPoints[finalPointsIter++] = points2D[i, j - 1];

                normals[finalPointsIter] = points2D[i - 1, j];
                finalPoints[finalPointsIter++] = points2D[i - 1, j];

                normals[finalPointsIter] = points2D[i, j];
                finalPoints[finalPointsIter++] = points2D[i, j];




                triangles[trianglesIter++] = finalPointsIter - 4;
                triangles[trianglesIter++] = finalPointsIter - 2;
                triangles[trianglesIter++] = finalPointsIter - 3;


                triangles[trianglesIter++] = finalPointsIter - 2;
                triangles[trianglesIter++] = finalPointsIter - 1;
                triangles[trianglesIter++] = finalPointsIter - 3;
            }
        Debug.Log(max);
        newMesh.vertices = finalPoints;
        newMesh.triangles = triangles; //= new int[triangles.Length];
        //for (int i = 0; i < triangles.Length; ++i)
        //    newMesh.triangles[i] = ;

        newMesh.normals = normals;

        //newMesh.RecalculateNormals();
        //newMesh.RecalculateTangents();

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
