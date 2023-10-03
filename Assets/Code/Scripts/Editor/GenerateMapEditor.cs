using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Adds a custom inspector to the GenerateMapScript class.
/// </summary>
[CustomEditor(typeof(GenerateMapScript))]
public class GenerateMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GenerateMapScript myScript = (GenerateMapScript)target;
        if (GUILayout.Button("Generate Map"))
        {
            myScript.DeleteMap();
            myScript.GenerateMap();
        }
        if (GUILayout.Button("Delete Map"))
        {
            myScript.DeleteMap();
        }
    }
}
