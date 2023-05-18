using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereChunkObjectPool : MonoBehaviour
{
    [SerializeField] GameObject sphereChunkPrefab;
    static SphereChunkObjectPool instance;
    private void Awake() => instance = this;

    static Queue<SphereChunk> sphereChunkPool = new();

    public static SphereChunk PopChunk()
    {
        if (sphereChunkPool.Count == 0)
            sphereChunkPool.Enqueue(GenerateChunk());

        return null;
    }
    static SphereChunk GenerateChunk()
    {
        SphereChunk newChunk = Instantiate(instance.sphereChunkPrefab, instance.transform).GetComponent<SphereChunk>();
        newChunk.enabled = false;


        return newChunk;
    }
    public static void PushChunk(SphereChunk chunk)
    {
        chunk.enabled = false;
        chunk.transform.SetParent(instance.transform);
        sphereChunkPool.Enqueue(chunk);
    }
}
