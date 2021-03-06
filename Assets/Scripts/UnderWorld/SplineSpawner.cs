﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class SplineSpawner : MonoBehaviour
{
    public GameObject ss;
    public GameObject platform;
    public GameObject particles;
    
    [Header("SPAWN PARAMS")]
    [SerializeField] float SpacingCoef = 6;
    [SerializeField] float StretchCoef = 4;

    Vector3 lastPosition = Vector3.zero;
    [SerializeField] List<GameObject> splineChunks;
    float count = 0;
    void Start()
    {
        ss.SetActive(false);
    }

    // Update is called once per frame
    public void AddObject(Transform parent)
    {
        float spacing = SpacingCoef + Random.Range(0,1f);
        float length = 1 + StretchCoef * Random.Range(0,1f);

        float r = Random.Range(-20f, 20f);
        float rz = spacing * Random.Range(0f, 1f);
        SplineLine sl = Instantiate(ss, lastPosition.withY(lastPosition.y - rz), Quaternion.Euler(0f, (360f / 4f) * (count%4) + r, 0f), parent).GetComponent<SplineLine>();
        sl.gameObject.SetActive(true);
        sl.gameObject.name = "Spline";
        lastPosition = sl.Init(100, length);
        count++;
  
        //every 8 lines, we add a platform, 
        if (count%5 == 4 && rz > 2f) {
            Vector3 endPos = sl.attachPos;
            Quaternion lookRot = Quaternion.LookRotation(endPos.withY(0).withX(endPos.x + Random.Range(-5,5f)).normalized);
            endPos = endPos.withY(endPos.y - 3.6f);
            GameObject pl = Instantiate(platform, endPos, lookRot, sl.gameObject.transform);
            pl.SetActive(true);
        }
        if(count %30 == 0){
            Debug.Log(sl.attachPos.y / Mathf.Floor(count/30));
        }
    }

    private int curChunkPos = 1;
    public int lastChunkPos = 1;
    public Transform sphere;
    public float w;
    public void Update() {
        // stop spawning new spline env if level is completed 
        if(UnderWorldLevelCode.instance.LevelCompleted) return;

        // calculate the current spline chunk index and activate if needed
        curChunkPos = Mathf.RoundToInt((CCPlayer.main.transform.position.y+30f) / -152f);
        if(curChunkPos < 2) return;
        if(lastChunkPos == curChunkPos) return;
        // reset last ones 
        splineChunks[lastChunkPos-1].SetActive(false);
        splineChunks[lastChunkPos].SetActive(false);
        splineChunks[lastChunkPos+1].SetActive(false);

        splineChunks[curChunkPos-1].SetActive(true);
        splineChunks[curChunkPos].SetActive(true);
        splineChunks[curChunkPos+1].SetActive(true);

        lastChunkPos = curChunkPos;
    }

    public void Clear() {
        foreach(var spline in splineChunks){
            DestroyImmediate(spline);
        }
        splineChunks.Clear();
        lastPosition = Vector3.zero;
        count = 0;
    }

    public void CreateSplineList() {
        if(splineChunks == null){
            splineChunks = new List<GameObject>();
        }
        Clear();
        for(int j = 0; j < 30; j++){
            // Instantiate
            GameObject splineChunkParent = new GameObject("SplineChunk");
            splineChunkParent.transform.SetParent(transform);

            GameObject particleGo = Instantiate(particles, lastPosition, Quaternion.identity, splineChunkParent.transform);
            particleGo.SetActive(true);

            for(int k = 0; k < 30; k++){
                AddObject(splineChunkParent.transform);
            }
            splineChunkParent.SetActive(false);
            splineChunks.Add(splineChunkParent);
        }
        for(int j = 0; j < 3; j++){
            splineChunks[j].SetActive(true);
        }
    }

    public void RemoveSplinesFromPos(float height, float dist)
    {
        bool criticalPointFound = false;
        for (int i = Mathf.Max(curChunkPos-1, 0); i <= curChunkPos+1 ; i++)
        {
            if(criticalPointFound)
            {
                splineChunks[i].SetActive(false);
                continue;
            }
            SplineLine[] splineLines = splineChunks[i].GetComponentsInChildren<SplineLine>();
            for (int j = 0; j < splineLines.Length; j++)
            {
                if(!criticalPointFound && splineLines[j].transform.position.y < height - dist) 
                {
                    criticalPointFound = true;
                }
                if(criticalPointFound)
                {
                    splineLines[j].gameObject.SetActive(false);
                }
            }
        }
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
        if(GUILayout.Button("Create Spline List"))
        {
            myScript.CreateSplineList();
        }
        if(GUILayout.Button("Clear"))
        {
            myScript.Clear();
        }
        if(GUILayout.Button("Refresh"))
        {
            
        }

    //                         //FOR DEBUGGING STRETCH EFFECT
    // Vector3 closestPoint = myScript.ss.GetComponent<BoxCollider>().ClosestPoint(myScript.sphere.position);
    // Debug.Log(closestPoint);
    // Vector3 dir = myScript.sphere.position - closestPoint;
    // myScript.ss.GetComponent<LineRenderer>().sharedMaterial.SetVector("_WarpDir", dir);
    // myScript.ss.GetComponent<LineRenderer>().sharedMaterial.SetVector(
    //                     "_WarpCenter",
    //                     new Vector4(closestPoint.x, closestPoint.y, closestPoint.z, myScript.w));
    }
}

#endif