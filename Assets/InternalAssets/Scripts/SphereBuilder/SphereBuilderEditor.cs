using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(SphereBuilder))]
[CanEditMultipleObjects]
public class SphereBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Regenerate"))
            ((SphereBuilder)target).CreateSphere();
    }
}
