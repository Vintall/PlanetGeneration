using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SphereChunk))]
public class SphereChunkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("SplitChunk"))
            ((SphereChunk)target).SplitChunk();

        if(GUILayout.Button("MergeSubChunks"))
            ((SphereChunk)target).MergeSubChunks();
    }
}
