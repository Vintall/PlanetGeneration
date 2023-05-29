using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;

public static class Noise
{
    struct NoiseChunk
    {
        public Vector3 basePointOffset;
        public Vector3 basePoint;
        public Vector3 endPoint;

        public Vector3[] edgePoints;
        public Vector3[] noisePoints;
    }
    //[BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Default, FloatPrecision = FloatPrecision.Standard, OptimizeFor = OptimizeFor.Performance)]
    //struct GenerateBasePointsJob : IJobFor
    //{
    //    [ReadOnly]
    //    public NativeArray<Vector3> points;

    //    public int span;

    //    [WriteOnly]
    //    public NativeArray<Vector3> basePoints;
    //    [WriteOnly]
    //    public NativeArray<NoiseChunk> noiseChunks;
    //    public void Execute(int index)
    //    {
    //        Vector3 currentPoint = points[index] + Vector3.one * 5000;

    //        Vector3 basePointOffset = new Vector3(currentPoint.x % span, currentPoint.y % span, currentPoint.z % span);
    //        Vector3 basePoint = currentPoint - basePointOffset;

    //        NoiseChunk noiseChunk = new NoiseChunk();
    //        noiseChunk.basePointOffset = basePointOffset;
    //        noiseChunk.basePoint = basePoint;
    //        noiseChunk.endPoint = basePoint + Vector3.one * span;
    //        noiseChunk.edgePoints = new UnsafeList<Vector3>(8, Allocator.Persistent);

    //        noiseChunk.edgePoints[0] = noiseChunk.basePoint;
    //        noiseChunk.edgePoints[1] = new Vector3(noiseChunk.endPoint.x, noiseChunk.basePoint.y, noiseChunk.basePoint.z);
    //        noiseChunk.edgePoints[2] = new Vector3(noiseChunk.basePoint.x, noiseChunk.endPoint.y, noiseChunk.basePoint.z);
    //        noiseChunk.edgePoints[3] = new Vector3(noiseChunk.basePoint.x, noiseChunk.basePoint.y, noiseChunk.endPoint.z);
    //        noiseChunk.edgePoints[4] = new Vector3(noiseChunk.endPoint.x, noiseChunk.endPoint.y, noiseChunk.basePoint.z);
    //        noiseChunk.edgePoints[5] = new Vector3(noiseChunk.endPoint.x, noiseChunk.basePoint.y, noiseChunk.endPoint.z);
    //        noiseChunk.edgePoints[6] = new Vector3(noiseChunk.basePoint.x, noiseChunk.endPoint.y, noiseChunk.endPoint.z);
    //        noiseChunk.edgePoints[7] = noiseChunk.endPoint;

    //        UnsafeList<Vector3> localNoisePoints = new UnsafeList<Vector3>(noiseChunk.edgePoints.Length, Allocator.Persistent);
    //        //float[] localpointsRanges = new float[edgePoints.Length];

    //        for (int j = 0; j < noiseChunk.edgePoints.Length; ++j)
    //        {
    //            uint xh = noiseChunk.edgePoints[j].x.ConvertTo<uint>();
    //            uint yh = noiseChunk.edgePoints[j].y.ConvertTo<uint>();
    //            uint zh = noiseChunk.edgePoints[j].z.ConvertTo<uint>();

    //            InitHash(xh + InitHash(yh + InitHash(zh)));

    //            Vector3 gradientVector = HashVector;
    //            localNoisePoints[j] = gradientVector;
    //        }
    //        noiseChunk.noisePoints = localNoisePoints;

    //        noiseChunks[index] = noiseChunk;
    //        basePoints[index] = basePoint;
    //    }
    //}
    public static float[] GeneratePoints(uint seed, int span, Vector3[] points)
    {
        float[] result = new float[points.Length];
        Dictionary<Vector3, NoiseChunk> noiseChunks = new();
        
        bool isBasePointExist = false;

        //GenerateBasePointsJob job = new GenerateBasePointsJob();

        //NativeArray<Vector3> pointsNative = new NativeArray<Vector3>(points.Length, Allocator.Persistent);
        //NativeArray<NoiseChunk> noiseChunksArray = new NativeArray<NoiseChunk>(points.Length, Allocator.Persistent);
        //NativeArray<Vector3> basePointsArray = new NativeArray<Vector3>(points.Length, Allocator.Persistent);

        //for (int i = 0; i < points.Length; ++i)
        //    pointsNative[i] = points[i];

        //job.points = pointsNative;
        //job.span = span;
        //job.noiseChunks = noiseChunksArray;
        //job.basePoints = basePointsArray;

        //job.Schedule(points.Length, new JobHandle()).Complete();

        //for (int i = 0; i < points.Length; ++i)
        //{
        //    if (noiseChunks.ContainsKey(job.basePoints[i]))
        //        continue;

        //    noiseChunks.Add(job.basePoints[i], job.noiseChunks[i]);
        //}

        //noiseChunks = job.noiseChunks;

        void GenerateBasePoints(Vector3 basePointOffset, Vector3 basePoint, Vector3 point)
        {
            NoiseChunk noiseChunk = new NoiseChunk();
            noiseChunk.basePointOffset = basePointOffset;
            noiseChunk.basePoint = basePoint;
            noiseChunk.endPoint = basePoint + Vector3.one * span;
            noiseChunk.edgePoints = new Vector3[8];//(8, Allocator.Persistent);

            noiseChunk.edgePoints[0] = noiseChunk.basePoint;
            noiseChunk.edgePoints[1] = new Vector3(noiseChunk.endPoint.x, noiseChunk.basePoint.y, noiseChunk.basePoint.z);
            noiseChunk.edgePoints[2] = new Vector3(noiseChunk.basePoint.x, noiseChunk.endPoint.y, noiseChunk.basePoint.z);
            noiseChunk.edgePoints[3] = new Vector3(noiseChunk.basePoint.x, noiseChunk.basePoint.y, noiseChunk.endPoint.z);
            noiseChunk.edgePoints[4] = new Vector3(noiseChunk.endPoint.x, noiseChunk.endPoint.y, noiseChunk.basePoint.z);
            noiseChunk.edgePoints[5] = new Vector3(noiseChunk.endPoint.x, noiseChunk.basePoint.y, noiseChunk.endPoint.z);
            noiseChunk.edgePoints[6] = new Vector3(noiseChunk.basePoint.x, noiseChunk.endPoint.y, noiseChunk.endPoint.z);
            noiseChunk.edgePoints[7] = noiseChunk.endPoint;

            Vector3[] localNoisePoints = new Vector3[noiseChunk.edgePoints.Length];
            

            for (int j = 0; j < noiseChunk.edgePoints.Length; ++j)
            {
                uint xh = noiseChunk.edgePoints[j].x.ConvertTo<uint>();
                uint yh = noiseChunk.edgePoints[j].y.ConvertTo<uint>();
                uint zh = noiseChunk.edgePoints[j].z.ConvertTo<uint>();

                HashingVector(xh, yh, zh, seed);

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
            DotGradient(localNoiseChunk.noisePoints[0], localNoiseChunk.edgePoints[0], currentPoint, span),
            DotGradient(localNoiseChunk.noisePoints[1], localNoiseChunk.edgePoints[1], currentPoint, span),
            DotGradient(localNoiseChunk.noisePoints[2], localNoiseChunk.edgePoints[2], currentPoint, span),
            DotGradient(localNoiseChunk.noisePoints[3], localNoiseChunk.edgePoints[3], currentPoint, span),
            DotGradient(localNoiseChunk.noisePoints[4], localNoiseChunk.edgePoints[4], currentPoint, span),
            DotGradient(localNoiseChunk.noisePoints[5], localNoiseChunk.edgePoints[5], currentPoint, span),
            DotGradient(localNoiseChunk.noisePoints[6], localNoiseChunk.edgePoints[6], currentPoint, span),
            DotGradient(localNoiseChunk.noisePoints[7], localNoiseChunk.edgePoints[7], currentPoint, span)
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

            result[i] = (Interpolate(firstPlateInterpolation, secondPlateInterpolation, zRelation) / 2 + 0.5f) / 0.8660f;
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
            uint xh = edgePoints[i].x.ConvertTo<uint>();
            uint yh = edgePoints[i].y.ConvertTo<uint>();
            uint zh = edgePoints[i].z.ConvertTo<uint>();

            HashingVector(xh, yh, zh, seed);

            Vector3 gradientVector = HashVector;
            noisePoints[i] = gradientVector;
            //pointsRanges[i] = 1 - (point - edgePoints[i]).magnitude / span;
            //accumulate += noisePoints[i] * pointsRanges[i];
        }

        float[] dotProduct = new float[]
        {
            DotGradient(noisePoints[0], edgePoints[0], point, span),
            DotGradient(noisePoints[1], edgePoints[1], point, span),
            DotGradient(noisePoints[2], edgePoints[2], point, span),
            DotGradient(noisePoints[3], edgePoints[3], point, span),
            DotGradient(noisePoints[4], edgePoints[4], point, span),
            DotGradient(noisePoints[5], edgePoints[5], point, span),
            DotGradient(noisePoints[6], edgePoints[6], point, span),
            DotGradient(noisePoints[7], edgePoints[7], point, span)
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
    static float DotGradient(Vector3 noiseVector, Vector3 gridPoint, Vector3 point, float span)
    {
        Vector3 delta = point - gridPoint;
        return Vector3.Dot(noiseVector, delta / span);
    }
    //static float Interpolate(float a, float b, float w) => a + (b - a) * w;
    static float Interpolate(float a, float b, float w) => (b - a) * (Mathf.Sin(Mathf.PI * (w - 0.5f)) / 2 + 0.5f) + a;

    static Unity.Mathematics.Random random;

    static uint HashingVector(uint x, uint y, uint z, uint seed) => InitHash(x + InitHash(y + InitHash(z + InitHash(seed))));
    static uint InitHash(uint seed)
    {
        random.InitState(seed);
        return random.NextUInt(0, 32769);
    }
    static Vector3 HashVector => random.NextFloat3(-1f, 1f);// new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

}
