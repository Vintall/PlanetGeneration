using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities.Graphics;
using Unity.Rendering;

public class SphereBuilder : MonoBehaviour
{
    [SerializeField, Range(1, 50)] 
    float radius = 1;

    [SerializeField, Range(5, 320)] 
    int pointsPerAxis;

    [SerializeField] 
    Material meshMaterial;

    [SerializeField, Header("Create when entered play mode")] 
    bool createOnStart;

    [SerializeField, Min(1)]
    float noiseSpan = 1;

    [SerializeField, Range(1, 7)]
    int noiseOctaves = 1;

    [SerializeField]
    Transform quadTreeControllPoint;

    List<SphereChunk> sphereChunks = new List<SphereChunk>(6);

    [BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low, OptimizeFor = OptimizeFor.Performance)]
    struct TestJob : IJobFor
    {
        [WriteOnly]
        public NativeArray<int> array;
        public void Execute(int index)
        {
            array[index] = index;
        }
    }
    void Start()
    {
        {
            //int[] asdf = new int[214748360];
            //double fTime = Time.realtimeSinceStartupAsDouble;
            //for (int i = 0; i < 214748360; ++i)
            //    asdf[i] = i;
            //double sTime = Time.realtimeSinceStartupAsDouble;
            //Debug.Log($"Common filling: {sTime - fTime}");
            //JobHandle handle = new JobHandle();
            //TestJob sdf = new TestJob()
            //{
            //    array = new NativeArray<int>(214748360, Allocator.Persistent)
            //};
            //fTime = Time.realtimeSinceStartupAsDouble;
            //sdf.Schedule(214748360, handle).Complete();
            //sTime = Time.realtimeSinceStartupAsDouble;
            //Debug.Log($"Jobs filling: {sTime - fTime}");
        } // Jobs Test

        if (createOnStart)
            CreateSphere();
    }
    private void Update()
    {
        QuadTreeLodUpdate();
    }
    void QuadTreeLodUpdate()
    {
        if (quadTreeControllPoint == null)
            return;

        if (sphereChunks.Count == 0)
            return;

        foreach (SphereChunk sphereChunk in sphereChunks)
            sphereChunk.QuadTreeLodUpdate(quadTreeControllPoint);
    }
    public void CreateSphere()
    {
        RemoveChilds();

        GameObject newSphere = new GameObject();
        newSphere.name = "Procedural Planet";

        Transform newSphereTransform = newSphere.transform;
        newSphereTransform.parent = transform;


        Vector2 сhunkSize = Vector2.one * radius * 2;

        SphereChunk.ChunkParams[] chunkParams = new SphereChunk.ChunkParams[6]
        {
            new SphereChunk.ChunkParams(Vector3.forward, Vector3.up, Vector3.right * radius, сhunkSize, radius), // Side Chunks
            new SphereChunk.ChunkParams(Vector3.left, Vector3.up, Vector3.forward * radius, сhunkSize, radius),
            new SphereChunk.ChunkParams(Vector3.back, Vector3.up, Vector3.left * radius, сhunkSize, radius),
            new SphereChunk.ChunkParams(Vector3.right, Vector3.up, Vector3.back * radius, сhunkSize, radius),
            new SphereChunk.ChunkParams(Vector3.right, Vector3.forward, Vector3.up * radius, сhunkSize, radius), // Top Plate
            new SphereChunk.ChunkParams(Vector3.left, Vector3.forward, Vector3.down * radius, сhunkSize, radius) // Bottom Plate
        };

        SphereChunk сhunkHolder;
        foreach (SphereChunk.ChunkParams chunkParam in chunkParams)
        {
            сhunkHolder = InstantiateChunk(chunkParam);
            сhunkHolder.transform.parent = newSphereTransform;
            сhunkHolder.gameObject.SetActive(true);
            sphereChunks.Add(сhunkHolder);
        }
    }
    void RemoveChilds()
    {
        for (int i = 0; i < sphereChunks.Count; ++i)
            SphereChunkObjectPool.PushChunk(sphereChunks[i]);

        sphereChunks.Clear();
    }
    SphereChunk InstantiateChunk(SphereChunk.ChunkParams chunkParams)
    {
        SphereChunk sphereChunk = SphereChunkObjectPool.PopChunk();
        sphereChunk.InstantiateChunk(chunkParams);
        return sphereChunk;
    }

    struct MeshPointsBuilderJob : IJobFor
    {
        [ReadOnly]
        public NativeArray<int> vertexes;
        [WriteOnly]
        public NativeArray<int> normals;
        public NativeArray<int> triangles;
        public void Execute(int index)
        {
            
        }
    }
    
}
