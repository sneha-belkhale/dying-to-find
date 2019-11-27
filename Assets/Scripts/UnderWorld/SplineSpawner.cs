using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class SplineSpawner : MonoBehaviour
{
    [SerializeField] GameObject ss;
    
    [Header("SPAWN PARAMS")]
    [SerializeField] float random = 360f;

    Vector3 lastPosition = Vector3.zero;
    List<SplineLine> splineList;
    float count = 0;
    void Start()
    {
        ss.SetActive(false);
        splineList = new List<SplineLine>();
        for(int j = 0; j < 30; j++){
            // Instantiate
            AddObject();
        }
    }

    // Update is called once per frame
    public void AddObject()
    {
        // float r = 2f * random * Mathf.PerlinNoise(count, 4f * Mathf.Sin(count/10f)) - random;
        float r = Random.Range(-20f, 20f);
        SplineLine sl = Instantiate(ss, lastPosition, Quaternion.Euler(0f, (360f / 4f) * (count%4) + r, 0f)).GetComponent<SplineLine>();
        sl.gameObject.SetActive(true);
        lastPosition = sl.Init();
        splineList.Add(sl);
        count++;

        //every 8 lines, we add a platform, 
        // if(count%5 == 4){
        //     Instantiate(platform, sl.endPos, Quaternion.identity);
        //     //instantiate 3 connecting lines

        // }
    }

    public void Clear() {
        foreach(var spline in splineList) {
            Destroy(spline.gameObject);
        }
        splineList.Clear();
        lastPosition = Vector3.zero;
        count = 0;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(SplineSpawner))]
public class SplineSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SplineSpawner myScript = (SplineSpawner)target;
        if(GUILayout.Button("Build Object"))
        {
            myScript.AddObject();
        }
        if(GUILayout.Button("Clear"))
        {
            myScript.Clear();
        }
    }
}

#endif