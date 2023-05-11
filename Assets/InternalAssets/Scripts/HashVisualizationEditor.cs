using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HashVisualization))]
public class HashVisualizationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Refresh field"))
            (target as HashVisualization).GenerateHash();
    }
}
