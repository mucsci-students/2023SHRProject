using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    
    private GUIStyle _style = new();
    private string money = "500";
    private string lives = "50";
    
    private void OnEnable()
    {
        _style.fontSize = 20;
        _style.fontStyle = FontStyle.BoldAndItalic;
        _style.normal.textColor = Color.white;
    }
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(20);
        GUILayout.Label("Cheats", _style);
        GUILayout.Space(5);
        
        GameManager myScript = (GameManager)target;

        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add Money", GUILayout.MinWidth(100), GUILayout.MaxWidth(200)))
        {
            try
            {
                myScript.AddMoney(int.Parse(money));
            } catch
            {
                Debug.Log("Invalid input");
            }
        }
        
        GUILayout.FlexibleSpace();

        money = GUILayout.TextField(money, 20, GUILayout.MinWidth(100), GUILayout.MaxWidth(400));
        
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add Lives", GUILayout.MinWidth(100), GUILayout.MaxWidth(200)))
        {
            try
            {
                myScript.AddLives(int.Parse(lives));
            } catch
            {
                Debug.Log("Invalid input");
            }
        }
        
        GUILayout.FlexibleSpace();
        
        lives = GUILayout.TextField(lives, 20, GUILayout.MinWidth(100), GUILayout.MaxWidth(400));
        
        GUILayout.EndHorizontal();
    }
}
