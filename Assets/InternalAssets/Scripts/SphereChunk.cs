using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereChunk : MonoBehaviour
{
    [SerializeField]
    MeshFilter meshFilter;

    [SerializeField]
    MeshRenderer meshRenderer;

    public enum SphereChunkMode
    {
        SingleChunk,
        SubChunk
    }
    SphereChunkMode sphereChunkMode = SphereChunkMode.SingleChunk;

    SphereChunk[] subChunks = new SphereChunk[4];
    SphereChunk parentChunk = null;
    const int maxSplittings = 7;
    int currentSplitting;

    Vector3[] cubeBounds = new Vector3[2];

    public void DestroyChunk()
    {

    }
    public void InstantiateChunk(Vector3 xVector, Vector3 yVector, Vector3 center, Vector2 size)
    {
        cubeBounds[0] = center - xVector * size.x / 2 - yVector * size.y / 2;
        cubeBounds[1] = center + xVector * size.x / 2 + yVector * size.y / 2;


    }
    void SplitChunk()
    {

    }
}
