using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereChunkObjectPool : MonoBehaviour
{
    [SerializeField] 
    GameObject sphereChunkPrefab;
    
    [SerializeField] 
    Transform inPoolHolder;

    [SerializeField]
    Transform temporalStorage;

    public static SphereChunkObjectPool instance;
    private void Awake()
    {
        instance = this;
        sphereChunkPool = new();
    }
    static Queue<SphereChunk> sphereChunkPool;

    public static SphereChunk PopChunk()
    {
        if (sphereChunkPool.Count == 0)
            sphereChunkPool.Enqueue(GenerateChunk());
        
        SphereChunk chunkBuffer = sphereChunkPool.Dequeue();
        chunkBuffer.transform.SetParent(instance.temporalStorage);

        return chunkBuffer;
    }
    static SphereChunk GenerateChunk()
    {
        SphereChunk newChunk = Instantiate(instance.sphereChunkPrefab, instance.inPoolHolder).GetComponent<SphereChunk>();
        return newChunk;
    }
    public static void PushChunk(SphereChunk chunk)
    {
        chunk.gameObject.SetActive(false);
        chunk.transform.SetParent(instance.inPoolHolder);
        sphereChunkPool.Enqueue(chunk);
    }
}
