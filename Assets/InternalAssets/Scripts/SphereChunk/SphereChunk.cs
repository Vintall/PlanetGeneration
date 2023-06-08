using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereChunk : MonoBehaviour
{
    [SerializeField]
    MeshFilter meshFilter;

    [SerializeField]
    MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        //meshRenderer.enabled = false;
    }

    public enum SphereChunkMode
    {
        SingleChunk,
        SubChunks
    }
    SphereChunkMode sphereChunkMode = SphereChunkMode.SingleChunk;
    ChunkParams chunkParams;

    List<SphereChunk> subChunks = new(4);

    const int maxSplittings = 6;
    int currentSplitting;
    uint seed = 0;

    
    private void Update()
    {
        //Graphics.DrawMesh(meshFilter.mesh, transform.position, Quaternion.identity, meshRenderer.material, 0);
    }

    public void DestroyChunk()
    {
        if (sphereChunkMode == SphereChunkMode.SubChunks)
        {
            foreach (SphereChunk sphereChunk in subChunks)
                sphereChunk.DestroyChunk();

            subChunks.Clear();
            sphereChunkMode = SphereChunkMode.SingleChunk;
        }
        SphereChunkObjectPool.PushChunk(this);
    }
    MeshBuilder.MeshParams meshParamsCopy;
    public void InstantiateChunk(ChunkParams chunkParams, MeshBuilder.MeshParams meshParams, uint seed)
    {
        this.seed = seed;
        meshParamsCopy = meshParams;
        this.chunkParams = chunkParams;
        Mesh newMesh = MeshBuilder.BuildPlaneMesh(chunkParams, meshParams, seed);
        
        meshRenderer.material.SetFloat("_Radius", chunkParams.radius);

        newMesh.RecalculateNormals();
        newMesh.RecalculateTangents();
        meshFilter.mesh = newMesh;
    }
    public void SplitChunk()
    {
        if (sphereChunkMode == SphereChunkMode.SubChunks)
            return;

        if (currentSplitting == maxSplittings)
            return;

        sphereChunkMode = SphereChunkMode.SubChunks;
        ChunkParams[] subChunksParams = chunkParams.SplitChunk();
         
        meshRenderer.enabled = false;
        //gameObject.SetActive(false);
        
        subChunks.Clear();
        for (int i = 0; i < 4; ++i)
        {
            SphereChunk buffer = SphereChunkObjectPool.PopChunk();
            
            buffer.InstantiateChunk(subChunksParams[i], meshParamsCopy, seed);
            buffer.transform.SetParent(transform);
            buffer.gameObject.SetActive(true); 
            subChunks.Add(buffer);
        }
    }
    public void MergeSubChunks()
    {
        if (sphereChunkMode == SphereChunkMode.SingleChunk)
            return;

        meshRenderer.enabled = true;
        sphereChunkMode = SphereChunkMode.SingleChunk;
        DestroySubChunks();
    }
    public void QuadTreeLodUpdate(Transform target)
    {
        Vector3 chunkCenterPosition = chunkParams.chunkCenter.normalized * chunkParams.radius + transform.position;
        float distanceToTarget = Vector3.Distance(target.position, chunkCenterPosition);
        if (distanceToTarget < chunkParams.chunkSize.x)
        {
            if (sphereChunkMode == SphereChunkMode.SingleChunk)
                SplitChunk();
            else
                foreach (SphereChunk sphereChunk in subChunks)
                    sphereChunk.QuadTreeLodUpdate(target);
        }
        else
        {
            if (sphereChunkMode == SphereChunkMode.SubChunks)
                MergeSubChunks();           
        }
    }
    void DestroySubChunks()
    {
        foreach (SphereChunk sphereChunk in subChunks)
            sphereChunk.DestroyChunk();

        //subChunks.Clear();
        sphereChunkMode = SphereChunkMode.SingleChunk;
    }
    public struct ChunkParams
    {
        public Vector3 xVector;
        public Vector3 yVector;
        public Vector3 chunkCenter;
        public Vector2 chunkSize;
        public float radius;

        public ChunkParams(Vector3 xVector, Vector3 yVector, Vector3 chunkCenter, Vector2 chunkSize, float radius)
        {
            this.xVector = xVector;
            this.yVector = yVector;
            this.chunkCenter = chunkCenter;
            this.chunkSize = chunkSize;
            this.radius = radius;
        }
        public ChunkParams(ChunkParams chunkParams)
        {
            xVector = chunkParams.xVector;
            yVector = chunkParams.yVector;
            chunkCenter = chunkParams.chunkCenter;
            chunkSize = chunkParams.chunkSize;
            radius = chunkParams.radius;
        }
        public ChunkParams[] SplitChunk()
        {
            ChunkParams[] result = new ChunkParams[4];
            for (int i = 0; i < 4; ++i)
                result[i] = new ChunkParams
                {
                    xVector = xVector,
                    yVector = yVector,
                    chunkSize = chunkSize / 2,
                    radius = radius
                };
            result[0].chunkCenter = chunkCenter - xVector * chunkSize.x / 4 - yVector * chunkSize.y / 4;
            result[1].chunkCenter = chunkCenter + xVector * chunkSize.x / 4 - yVector * chunkSize.y / 4;
            result[2].chunkCenter = chunkCenter - xVector * chunkSize.x / 4 + yVector * chunkSize.y / 4;
            result[3].chunkCenter = chunkCenter + xVector * chunkSize.x / 4 + yVector * chunkSize.y / 4;

            return result;
        }
    }
}
