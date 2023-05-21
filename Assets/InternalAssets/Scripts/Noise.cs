using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;

public static class Noise
{
    struct NoiseChunk
    {
        public Vector3 basePointOffset;
        public Vector3 basePoint;
        public Vector3 endPoint;
        public Vector3[] edgePoints;
        public Vector3[] noisePoints;
        public float[] pointsRanges;
    }
    public static float[] GeneratePoints(uint seed, int span, Vector3[] points)
    {
        float[] result = new float[points.Length];
        //for (int i = 0; i < points.Length; ++i)
        //    result[i] = GeneratePoint(seed, span, points[i]);

        //return result;

        Dictionary<Vector3, NoiseChunk> noiseChunks = new(50);
        
        bool isBasePointExist = false;

        void GenerateBasePoints(Vector3 basePointOffset, Vector3 basePoint, Vector3 point)
        {
            NoiseChunk noiseChunk = new NoiseChunk();
            noiseChunk.basePointOffset = basePointOffset;
            noiseChunk.basePoint = basePoint;
            noiseChunk.endPoint = basePoint + Vector3.one * span;
            noiseChunk.edgePoints = new Vector3[]
            {
                noiseChunk.basePoint,
                new Vector3(noiseChunk.endPoint.x, noiseChunk.basePoint.y, noiseChunk.basePoint.z),
                new Vector3(noiseChunk.basePoint.x, noiseChunk.endPoint.y, noiseChunk.basePoint.z),
                new Vector3(noiseChunk.basePoint.x, noiseChunk.basePoint.y, noiseChunk.endPoint.z),
                new Vector3(noiseChunk.endPoint.x, noiseChunk.endPoint.y, noiseChunk.basePoint.z),
                new Vector3(noiseChunk.endPoint.x, noiseChunk.basePoint.y, noiseChunk.endPoint.z),
                new Vector3(noiseChunk.basePoint.x, noiseChunk.endPoint.y, noiseChunk.endPoint.z),
                noiseChunk.endPoint
            };
            Vector3[] localNoisePoints = new Vector3[noiseChunk.edgePoints.Length];
            //float[] localpointsRanges = new float[edgePoints.Length];

            for (int j = 0; j < noiseChunk.edgePoints.Length; ++j)
            {
                InitHash((int)noiseChunk.edgePoints[j].x + InitHash((int)noiseChunk.edgePoints[j].y + InitHash((int)noiseChunk.edgePoints[j].z)));

                Vector3 gradientVector = HashVector;
                localNoisePoints[j] = gradientVector;
            }
            noiseChunk.noisePoints = localNoisePoints;

            noiseChunks.Add(basePoint, noiseChunk);
        }
        Vector3 currentPoint;
        for (int i = 0; i < points.Length; ++i)
        {
            currentPoint = points[i] + Vector3.one * 5000;
            Vector3 basePointOffset = new Vector3(currentPoint.x % span, currentPoint.y % span, currentPoint.z % span);
            Vector3 basePoint = currentPoint - basePointOffset;

            if (!noiseChunks.ContainsKey(basePoint))
                GenerateBasePoints(basePointOffset, basePoint, currentPoint);

            NoiseChunk localNoiseChunk = noiseChunks[basePoint];
            float[] dotProduct = new float[]
            {
            DotGradient(ref localNoiseChunk.noisePoints[0], ref localNoiseChunk.edgePoints[0], ref currentPoint, span),
            DotGradient(ref localNoiseChunk.noisePoints[1], ref localNoiseChunk.edgePoints[1], ref currentPoint, span),
            DotGradient(ref localNoiseChunk.noisePoints[2], ref localNoiseChunk.edgePoints[2], ref currentPoint, span),
            DotGradient(ref localNoiseChunk.noisePoints[3], ref localNoiseChunk.edgePoints[3], ref currentPoint, span),
            DotGradient(ref localNoiseChunk.noisePoints[4], ref localNoiseChunk.edgePoints[4], ref currentPoint, span),
            DotGradient(ref localNoiseChunk.noisePoints[5], ref localNoiseChunk.edgePoints[5], ref currentPoint, span),
            DotGradient(ref localNoiseChunk.noisePoints[6], ref localNoiseChunk.edgePoints[6], ref currentPoint, span),
            DotGradient(ref localNoiseChunk.noisePoints[7], ref localNoiseChunk.edgePoints[7], ref currentPoint, span)
            };

            float xRelation = (currentPoint.x - localNoiseChunk.edgePoints[0].x) / span;
            float yRelation = (currentPoint.y - localNoiseChunk.edgePoints[0].y) / span;
            float zRelation = (currentPoint.z - localNoiseChunk.edgePoints[0].z) / span;

            xRelation *= xRelation;
            yRelation *= yRelation;
            zRelation *= zRelation;

            float firstPlateInterpolation = Interpolate(
                Interpolate(dotProduct[0], dotProduct[1], xRelation),
                Interpolate(dotProduct[2], dotProduct[4], xRelation),
                yRelation);

            float secondPlateInterpolation = Interpolate(
                Interpolate(dotProduct[3], dotProduct[5], xRelation),
                Interpolate(dotProduct[6], dotProduct[7], xRelation),
                yRelation);

            result[i] = Interpolate(firstPlateInterpolation, secondPlateInterpolation, zRelation) / 2 + 0.5f;
        }

        return result;
    }
    public static float GeneratePoint(uint seed, int span, Vector3 point)
    {
        point += Vector3.one * 5000;
        Vector3 basePointOffset = new Vector3(point.x % span, point.y % span, point.z % span);
        Vector3 basePoint = point - basePointOffset;

        Vector3 endPoint = basePoint + Vector3.one * span;

        Vector3[] edgePoints = new Vector3[]
        {
            basePoint,
            new Vector3(endPoint.x, basePoint.y, basePoint.z),
            new Vector3(basePoint.x, endPoint.y, basePoint.z),
            new Vector3(basePoint.x, basePoint.y, endPoint.z),
            new Vector3(endPoint.x, endPoint.y, basePoint.z),
            new Vector3(endPoint.x, basePoint.y, endPoint.z),
            new Vector3(basePoint.x, endPoint.y, endPoint.z),
            endPoint
        };
        Vector3[] noisePoints = new Vector3[edgePoints.Length];
        float[] pointsRanges = new float[edgePoints.Length];

        for (int i = 0; i < edgePoints.Length; ++i)
        {
            InitHash((int)edgePoints[i].x + InitHash((int)edgePoints[i].y + InitHash((int)edgePoints[i].z)));

            Vector3 gradientVector = HashVector;
            noisePoints[i] = gradientVector;
            //pointsRanges[i] = 1 - (point - edgePoints[i]).magnitude / span;
            //accumulate += noisePoints[i] * pointsRanges[i];
        }

        float[] dotProduct = new float[]
        {
            DotGradient(ref noisePoints[0], ref edgePoints[0], ref point, span),
            DotGradient(ref noisePoints[1], ref edgePoints[1], ref point, span),
            DotGradient(ref noisePoints[2], ref edgePoints[2], ref point, span),
            DotGradient(ref noisePoints[3], ref edgePoints[3], ref point, span),
            DotGradient(ref noisePoints[4], ref edgePoints[4], ref point, span),
            DotGradient(ref noisePoints[5], ref edgePoints[5], ref point, span),
            DotGradient(ref noisePoints[6], ref edgePoints[6], ref point, span),
            DotGradient(ref noisePoints[7], ref edgePoints[7], ref point, span)
        };

        float xRelation = (point.x - edgePoints[0].x) / span;
        float yRelation = (point.y - edgePoints[0].y) / span;
        float zRelation = (point.z - edgePoints[0].z) / span;

        xRelation *= xRelation;
        yRelation *= yRelation;
        zRelation *= zRelation;

        float firstPlateInterpolation = Interpolate(
            Interpolate(dotProduct[0], dotProduct[1], xRelation),
            Interpolate(dotProduct[2], dotProduct[4], xRelation),
            yRelation);

        float secondPlateInterpolation = Interpolate(
            Interpolate(dotProduct[3], dotProduct[5], xRelation),
            Interpolate(dotProduct[6], dotProduct[7], xRelation),
            yRelation);

        float fullInterpolation = Interpolate(firstPlateInterpolation, secondPlateInterpolation, zRelation);

        return fullInterpolation / 2 + 0.5f;
    }
    static float DotGradient(ref Vector3 noiseVector, ref Vector3 gridPoint, ref Vector3 point, float span)
    {
        Vector3 delta = point - gridPoint;
        return Vector3.Dot(noiseVector, delta / span);
    }
    //static float Interpolate(float a, float b, float w) => a + (b - a) * w;
    static float Interpolate(float a, float b, float w) => (b - a) * (Mathf.Sin(Mathf.PI * (w - 0.5f)) / 2 + 0.5f) + a;

    static int InitHash(int seed)
    {
        Random.InitState(seed);
        return Random.Range(0, 32769);
    }
    static Vector3 HashVector => new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

}
