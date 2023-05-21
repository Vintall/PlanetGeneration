using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Noise
{
    public static float[,] GeneratePoints(uint seed, float span, Vector3[] points, )
    {
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
            InitHash((int)edgePoints[i].x + InitHash((int)edgePoints[i].y) + InitHash(InitHash((int)edgePoints[i].z)));

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
    public static float GeneratePoint(uint seed, int span, Vector3 point)
    {
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
