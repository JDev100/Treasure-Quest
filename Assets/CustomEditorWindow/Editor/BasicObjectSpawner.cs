using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BasicObjectSpawner : EditorWindow
{
    string objectBaseName = "";
    int objectID = 1;
    GameObject objectToSpawn;
    float objectScale;
    float spawnRadius = 5f;

    Transform objectContainer;
    bool appendID;
    float minScaleVal = 1f;
    float maxScaleVal = 3f;
    float minScaleLimit = 0.5f;
    float maxScaleLimit = 5f;

    [MenuItem("Tools/Basic Object Spawner")]
    public static void ShowWindow()
    {
        GetWindow(typeof(BasicObjectSpawner));
    }

    private void OnGUI()
    {
        GUILayout.Label("Spawn New Object", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        objectToSpawn = EditorGUILayout.ObjectField("Prefab to Spawn", objectToSpawn, typeof(GameObject), false) as GameObject;

        objectContainer = EditorGUILayout.ObjectField("Object Container", objectContainer, typeof(Transform), true) as Transform;
        EditorGUILayout.HelpBox("Object Container not requiered", MessageType.None, false);

        EditorGUILayout.Space();

        objectBaseName = EditorGUILayout.TextField("Base Name", objectBaseName);

        appendID = EditorGUILayout.BeginToggleGroup("Append Numerical ID", appendID);
        EditorGUI.indentLevel++;
        objectID = EditorGUILayout.IntField("Object ID", objectID);
        EditorGUI.indentLevel--;
        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.Space();

        spawnRadius = EditorGUILayout.FloatField("Spawn Radius", spawnRadius);

        EditorGUILayout.Space();

        GUILayout.Label("Object Scale");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Min Limit: " + minScaleLimit);
        EditorGUILayout.MinMaxSlider(ref minScaleVal, ref maxScaleVal, minScaleLimit, maxScaleLimit);
        EditorGUILayout.PrefixLabel("Max Limit: " + maxScaleLimit);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Min Value: " + minScaleVal.ToString());
        EditorGUILayout.LabelField("Max Value: " + maxScaleVal.ToString());
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(objectToSpawn == null ||
                                    (objectContainer != null && EditorUtility.IsPersistent(objectContainer)));


        if (GUILayout.Button("Spawn Object"))
        {
            SpawnObject();
        }

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();

        if (objectToSpawn == null)
        {
            EditorGUILayout.HelpBox("Place a GameObject in the 'Prefab to Spawn' field.", MessageType.Warning);
        }


    }

    void SpawnObject()
    {
        string baseName = objectBaseName;
        if (objectBaseName == string.Empty)
        {
            baseName = objectToSpawn.name;
        }

        Vector2 spawnCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = new Vector3(spawnCircle.x, spawnCircle.y);
        objectScale = Random.Range(minScaleVal, maxScaleVal);

        if (appendID)
        {
            baseName += objectID.ToString();
            objectID++;
        }

        GameObject newObject = Instantiate(objectToSpawn, spawnPos, Quaternion.identity);
        newObject.name = baseName;
        newObject.transform.localScale = Vector3.one * objectScale;

    }
}
