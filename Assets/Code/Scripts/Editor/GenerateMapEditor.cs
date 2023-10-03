using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
