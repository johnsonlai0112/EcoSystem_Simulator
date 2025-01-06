using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class EcosystemWindow : EditorWindow
{
    private GameObject prefab;
    private float speed;
    private float fov;

    [MenuItem("Window/Ecosystem Window")]
    public static void ShowWindow()
    {
        GetWindow<EcosystemWindow>("Prefab Attributes");
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab Attributes", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        speed = EditorGUILayout.FloatField("Speed", speed);
        fov = EditorGUILayout.FloatField("Field of View", fov);

        if (GUILayout.Button("Generate Object"))
        {
            GenerateObject();
        }
    }

    private void GenerateObject()
    {
        if (prefab != null)
        {
            GameObject newObject = Instantiate(prefab);
            // Set attributes like speed and FOV
            // Example: newObject.GetComponent<YourScript>().SetSpeed(speed);
            //          newObject.GetComponent<YourScript>().SetFOV(fov);
        }
        else
        {
            Debug.LogWarning("Prefab not assigned.");
        }
    }
}
