using UnityEngine;
using UnityEditor;

/// <summary>
/// Adds a custom inspector to the GameManager class.
/// </summary>
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    
    private readonly GUIStyle _style = new();
    private string _money = "500";
    private string _lives = "50";
    
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
                myScript.AddMoney(int.Parse(_money));
            } catch
            {
                Debug.Log("Invalid input");
            }
        }
        
        GUILayout.FlexibleSpace();

        _money = GUILayout.TextField(_money, 20, GUILayout.MinWidth(100), GUILayout.MaxWidth(400));
        
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add Lives", GUILayout.MinWidth(100), GUILayout.MaxWidth(200)))
        {
            try
            {
                myScript.AddLives(int.Parse(_lives));
            } catch
            {
                Debug.Log("Invalid input");
            }
        }
        
        GUILayout.FlexibleSpace();
        
        _lives = GUILayout.TextField(_lives, 20, GUILayout.MinWidth(100), GUILayout.MaxWidth(400));
        
        GUILayout.EndHorizontal();
    }
}
