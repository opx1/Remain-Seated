using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ObjectFillerTool : EditorWindow
{
    public GameObject startObject;
    public GameObject endObject;
    public GameObject ObjectPrefab;
    public int divisions = 1;

    private ObjectLength objectMainLength;

    private float prefabMainLength;

    private enum ObjectLength
    {
        X,
        Y,
        Z
    }

    [MenuItem("Tools/Object Filler (Tile)")]
    static void Open()
    {
        GetWindow<ObjectFillerTool>("Object Filler");
    }

    void OnGUI()
    {
        GUILayout.Label("Object Filling Tool", EditorStyles.boldLabel);

        startObject = (GameObject)EditorGUILayout.ObjectField("Start Object", startObject, typeof(GameObject), true);
        endObject = (GameObject)EditorGUILayout.ObjectField("End Object", endObject, typeof(GameObject), true);
        ObjectPrefab = (GameObject)EditorGUILayout.ObjectField("Object Prefab", ObjectPrefab, typeof(GameObject), false);

        divisions = Mathf.Max(1, EditorGUILayout.IntField("Divisions", divisions));
        objectMainLength = (ObjectLength)EditorGUILayout.EnumPopup("Object Length Axis", objectMainLength);

        GUILayout.Space(10);

        if (GUILayout.Button("Tile Objects"))
            TileObjects();

        GUILayout.Space(10);

        GUILayout.Label("Quick Actions", EditorStyles.boldLabel);

        if (GUILayout.Button("Reset Positions"))
        {
            startObject.transform.localPosition = Vector3.zero;
            endObject.transform.localPosition = Vector3.zero;
            Selection.activeGameObject = endObject.transform.parent.gameObject;
        }

        if (GUILayout.Button("Reset Positions (On Left)"))
        {
            Vector3 resetPos = endObject.transform.position;

            endObject.transform.parent.localPosition = resetPos;
            startObject.transform.localPosition = Vector3.zero;
            endObject.transform.localPosition = Vector3.zero;

            Selection.activeGameObject = endObject.transform.parent.gameObject;
        }

        if (GUILayout.Button("Select Tool"))
        {
            Selection.activeGameObject = endObject.transform.parent.gameObject;
        }

        if (GUILayout.Button("Select Right"))
        {
            Selection.activeGameObject = startObject;
        }

        if (GUILayout.Button("Select Left"))
        {
            Selection.activeGameObject = endObject;
        }


    }

    void TileObjects()
    {
        if (!startObject || !endObject || !ObjectPrefab)
        {
            Debug.LogError("Missing references");
            return;
        }

        CachePrefabLength();

        Vector3 start = startObject.transform.position;
        Vector3 end = endObject.transform.position;

        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        float segmentLength = distance / divisions;

        Quaternion rotation = Quaternion.LookRotation(direction);

        for (int i = 0; i < divisions; i++)
        {
            Vector3 spawnPos = start + direction * (i * segmentLength);

            GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(ObjectPrefab);
            Undo.RegisterCreatedObjectUndo(newObj, "Tile Object");

            newObj.transform.position = spawnPos;
            newObj.transform.rotation = rotation;

            float scaleFactor = segmentLength / prefabMainLength;

            Vector3 scale = newObj.transform.localScale;

            switch (objectMainLength)
            {
                case ObjectLength.X:
                    scale.x *= scaleFactor;
                    break;
                case ObjectLength.Y:
                    scale.y *= scaleFactor;
                    break;
                case ObjectLength.Z:
                    scale.z *= scaleFactor;
                    break;
            }

            newObj.transform.localScale = scale;
            PrefabUtility.UnpackPrefabInstance(newObj, PrefabUnpackMode.Completely, InteractionMode.UserAction);
        }
    }

    void CachePrefabLength()
    {
        GameObject temp = (GameObject)PrefabUtility.InstantiatePrefab(ObjectPrefab);

        MeshRenderer mr = temp.GetComponentInChildren<MeshRenderer>();
        if (!mr)
        {
            Debug.LogError("Prefab has no MeshRenderer");
            DestroyImmediate(temp);
            return;
        }

        switch (objectMainLength)
        {
            case ObjectLength.X:
                prefabMainLength = mr.bounds.size.x;
                break;
            case ObjectLength.Y:
                prefabMainLength = mr.bounds.size.y;
                break;
            case ObjectLength.Z:
                prefabMainLength = mr.bounds.size.z;
                break;
        }

        DestroyImmediate(temp);
    }
}