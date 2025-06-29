using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AddSheetPopup : EditorWindow
{
    private string sheetURL = "";
    private string sheetName = "";
    private string className = "";

    private List<string> sheetURLs;
    private List<string> sheetTabNames;
    private List<string> classNames;

    public static void Open(List<string> sheetURLs, List<string> sheetTabNames, List<string> classNames)
    {
        var window = CreateInstance<AddSheetPopup>();
        window.sheetURLs = sheetURLs;
        window.sheetTabNames = sheetTabNames;
        window.classNames = classNames;
        window.titleContent = new GUIContent("Add Sheet");
        window.position = new Rect(Screen.width / 2f - 200, Screen.height / 2f - 75, 400, 150);
        window.ShowUtility();
    }

    private void OnGUI()
    {
        GUILayout.Label("Add Google Sheet", EditorStyles.boldLabel);
        sheetURL = EditorGUILayout.TextField("Sheet URL", sheetURL);
        sheetName = EditorGUILayout.TextField("Sheet Name", sheetName);
        className = EditorGUILayout.TextField("Class Name", className);

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            sheetURLs.Add(sheetURL);
            sheetTabNames.Add(sheetName);
            classNames.Add(className);
            GoogleSheetLoader.SavePrefs(sheetURLs, sheetTabNames, classNames);
            Close();
        }
        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
        EditorGUILayout.EndHorizontal();
    }
}