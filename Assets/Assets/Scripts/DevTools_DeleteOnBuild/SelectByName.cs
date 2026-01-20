using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SelectByName : EditorWindow
{
    public string nameSubstring = "";
    public GameObject[] selectedObjects;

    [MenuItem("Tools/Select By Name")]
    static void Open()
    {
        GetWindow<SelectByName>("Select By Name");
    }

    void OnGUI()
    {
        GUILayout.Label("Select Objects By Name", EditorStyles.boldLabel);

        nameSubstring = EditorGUILayout.TextField("Name Substring", nameSubstring);

        if (GUILayout.Button("Select Objects"))
            SelectObjectsByName();
    }

    void SelectObjectsByName()
    {
        if (string.IsNullOrEmpty(nameSubstring))
        {
            Debug.LogError("Name substring cannot be empty");
            return;
        }

        List<GameObject> allObjects = new List<GameObject>(FindObjectsByType<GameObject>(FindObjectsSortMode.None));
        var selectedList = new List<GameObject>();

        foreach (var obj in allObjects)
        {
            if (obj.name.Contains(nameSubstring))
            {
                selectedList.Add(obj);
            }
        }

        selectedObjects = selectedList.ToArray();
        Selection.objects = selectedObjects;

        Debug.Log($"Selected {selectedObjects.Length} objects containing '{nameSubstring}' in their names.");
    }
}
