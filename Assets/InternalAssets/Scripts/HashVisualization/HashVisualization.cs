using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;

using static Unity.Mathematics.math;
using Unity.Entities.UniversalDelegates;

public class HashVisualization : MonoBehaviour
{
    [SerializeField]
    GameObject primitive;

    [SerializeField]
    Material material;

    [SerializeField, Range(1, 512)]
    int resolution = 16;

    [SerializeField, Range(1, 10)]
    float amplitudeMultiplier = 1;

    [SerializeField, Range(1, 32)]
    int scale = 1;

    bool playmodeCheck = false;

    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    struct HashJob : IJobFor
    {
        [WriteOnly]
        public NativeArray<uint> hashes;

        public void Execute(int index)
        {
            hashes[index] = (uint)index;

        }
    }
    private void Start()
    {
        playmodeCheck = true;
        GenerateHash();
    }
    void RemoveChilds()
    {
        for (int i = 0; i < transform.childCount; ++i)
            Destroy(transform.GetChild(i).gameObject);
    }
    public void GenerateHash()
    {
        RemoveChilds();
        Vector3 position;
        

        float[,] heights = new float[resolution, resolution];

        float fullScale = (float)scale / resolution;
        float max = 0, min = 1000;
        for (int x = 0; x < resolution; ++x)
            for (int z = 0; z < resolution; ++z)
            {
                Transform cube = Instantiate(primitive).transform;
                position = Vector3.zero;
                position.x = (float)x * fullScale + fullScale * 0.5f;
                position.z = (float)z * fullScale + fullScale * 0.5f;

                float noiseResult = Noise.GeneratePoint(0, 10, position);
                position.y = noiseResult * amplitudeMultiplier;
                cube.name = noiseResult.ToString();

                if (noiseResult > max)
                    max = noiseResult;
                if (noiseResult < min)
                    min = noiseResult;

                cube.position = position;
                cube.localScale = Vector3.one * fullScale;
                cube.SetParent(transform);
            }
        Debug.Log($"Max: {max}");
        Debug.Log($"Min: {min}");

    }
    private void OnEnable()
    {
        HashJob job = new HashJob
        {

        };
        //job.ScheduleParallel(hashes.Length, resolution, default).Complete();
    }
}
