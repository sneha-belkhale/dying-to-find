using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class CrowdScatterWindow : EditorWindow
{
    [SerializeField] Object objectToScatter;
    [SerializeField] Object scatterAreaField;
    [SerializeField] List<Object> scatteredObjects;

    [SerializeField] static float step = 2.0f;

    [MenuItem("Tools/Crowd Scatter")]
    static void OpenWindow()
    {
        CrowdScatterWindow window = (CrowdScatterWindow)GetWindow(typeof(CrowdScatterWindow));
        window.minSize = new Vector2(300, 300);
        window.Show();
    }

    public void OnGUI()
    {
        GUILayout.Label("Object To Scatter", EditorStyles.label);
        objectToScatter = EditorGUILayout.ObjectField(objectToScatter, typeof(GameObject), true);

        GUILayout.Label("Scatter Area", EditorStyles.label);
        scatterAreaField = EditorGUILayout.ObjectField(scatterAreaField, typeof(BoxCollider), true);

        GUILayout.Label("Step", EditorStyles.label);
        step = EditorGUILayout.Slider(step, 0.001f, 100.0f);

        if (GUILayout.Button("Generate"))
        {
            if (scatteredObjects == null)
            {
                scatteredObjects = new List<Object>();
            }

            GameObject group = new GameObject("Crowd");
            scatteredObjects.Add(group);

            BoxCollider scatterArea = (BoxCollider)scatterAreaField;
            GameObject objToScatter = (GameObject)objectToScatter;
            Vector3 pos = scatterArea.bounds.center;
            Vector3 size = scatterArea.bounds.size;

            float startX = pos.x - (size.x / 2.0f);
            float endX = pos.x + (size.x / 2.0f);
            float startZ = pos.z - (size.z / 2.0f);
            float endZ = pos.z + (size.z / 2.0f);

            int count = 0;

            for (float x = startX; x < endX; x += step)
            {
                for (float z = startZ; z < endZ; z += step)
                {
                    float rMin = -step;
                    float rMax = step;
                    float rX = Random.Range(rMin, rMax);
                    float rZ = Random.Range(rMin, rMax);
                    float rScale = Random.Range(1f, 1.2f);

                    Vector3 p = new Vector3(x + rX, 0.0f, z + rZ);

                    GameObject obj = Instantiate(objToScatter);
                    scatteredObjects.Add(obj);

                    obj.transform.parent = group.transform;

                    NavMeshAgent nmAgent = obj.GetComponent<NavMeshAgent>();
                    nmAgent.Warp(p);

                    // Randomize scale
                    Vector3 ls = obj.transform.localScale;
                    obj.transform.localScale = new Vector3(ls.x * rScale, ls.y * rScale, ls.z * rScale);

                    count++;
                }
            }

            Debug.Log("Total agents " + count);
        }

        if (GUILayout.Button("Clear"))
        {
            foreach (Object obj in scatteredObjects)
            {
                DestroyImmediate(obj);
            }
        }
    }
}
