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

    [SerializeField, Header("First triange out of quad")] 
    bool fillFirstTriangle;

    [SerializeField, Header("Second triange out of quad")] 
    bool fillSecondTriangle;

    [SerializeField, Tooltip("Obsolete")] 
    bool fillBackwardTriangle;

    [SerializeField, Range(0f, 1f), Header("0/1 - cube/sphere")] 
    float interpolationValue;

    [SerializeField, Min(1)]
    float noiseSpan = 1;

    [SerializeField, Range(1, 7)]
    int noiseOctaves = 1;


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
    struct PlaneParams
    {
        public Vector3 xVector;
        public Vector3 yVector;
        public Vector3 planeCenter;
        public Vector2 planeSize;

        public PlaneParams(Vector3 xVector, Vector3 yVector, Vector3 planeCenter, Vector2 planeSize)
        {
            this.xVector = xVector;
            this.yVector = yVector;
            this.planeCenter = planeCenter;
            this.planeSize = planeSize;
        }
    }
    public void CreateSphere()
    {
        RemoveChilds();

        GameObject newSphere = new GameObject();
        Transform newSphereTransform = newSphere.transform;
        newSphereTransform.parent = transform;

        Transform planeHolder;

        Vector2 planeSize = Vector2.one * radius * 2;

        PlaneParams[] planeParams = new PlaneParams[6]
        {
            new PlaneParams(Vector3.forward, Vector3.up, Vector3.right * radius, planeSize), // Side planes
            new PlaneParams(Vector3.left, Vector3.up, Vector3.forward * radius, planeSize),
            new PlaneParams(Vector3.back, Vector3.up, Vector3.left * radius, planeSize),
            new PlaneParams(Vector3.right, Vector3.up, Vector3.back * radius, planeSize),
            new PlaneParams(Vector3.right, Vector3.forward, Vector3.up * radius, planeSize), // Top Plate
            new PlaneParams(Vector3.left, Vector3.forward, Vector3.down * radius, planeSize) // Bottom Plate
        };

        foreach (PlaneParams planeParam in planeParams)
        {
            planeHolder = InstantiatePlane(planeParam.xVector, planeParam.yVector, planeParam.planeCenter, planeParam.planeSize);
            planeHolder.parent = newSphereTransform;
        }
    }
    Transform InstantiatePlane(Vector3 xVector, Vector3 yVector, Vector3 center, Vector2 size)
    {
        //SphereChunk sphereChunk = SphereChunkObjectPool.PopChunk();
        //sphereChunk.InstantiateChunk(xVector, yVector, center, size);

        Mesh newMesh = MeshBuilder.BuildPlaneMesh(xVector, yVector, center, size);
        newMesh.RecalculateNormals();
        newMesh.RecalculateTangents();

        GameObject plane = new GameObject();
        MeshFilter meshFilter = plane.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = plane.AddComponent<MeshRenderer>();

        meshFilter.mesh = newMesh;
        meshRenderer.material = meshMaterial;
        return plane.transform;
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
    void RemoveChilds()
    {
        for (int i = 0; i < transform.childCount; ++i)
            Destroy(transform.GetChild(i).gameObject);
    }
}
