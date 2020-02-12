using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
[ExecuteInEditMode]
public class SplineSpawnerTest : MonoBehaviour
{
    public GameObject ss;
    public GameObject platform;
    
    [Header("SPAWN PARAMS")]
    [SerializeField] float SpacingCoef = 6;
    [SerializeField] float StretchCoef = 4;
    [SerializeField] float MergeDistance = 250f;
    [SerializeField] float MergeStrength = 250f;
    [SerializeField] float SmoothingRange = 20f;
    [SerializeField] int Iterations = 20;

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
        float length = 1 + StretchCoef + Random.Range(0,0.2f*StretchCoef);

        float r = Random.Range(0f, 180f);
        float rz = spacing * Random.Range(0.75f, 1f);
        lastPosition = Quaternion.Euler(0, r, 0) * lastPosition;
        Quaternion forward = Quaternion.Euler(0f,r, 0);
        SplineLineTest sl = Instantiate(ss, lastPosition.withY(lastPosition.y - rz) - forward * (50f * Vector3.forward), forward, parent).GetComponent<SplineLineTest>();
        sl.gameObject.SetActive(true);
        sl.gameObject.name = "Spline";
        lastPosition.y -= 0.5f * spacing;
        sl.Init(length);
        count++;
  
        if (count%5 == 4) {
            GameObject pl = Instantiate(platform, sl.attachPos, Quaternion.identity, sl.gameObject.transform);
            pl.SetActive(true);
        }
    }

    private int curChunkPos = 1;
    private int lastChunkPos = 1;
    public Transform sphere;
    public float w;
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
        for(int j = 0; j < 1; j++){
            // Instantiate
            GameObject splineChunkParent = new GameObject("SplineChunk");
            splineChunkParent.transform.SetParent(transform);
            for(int k = 0; k < 30; k++){
                AddObject(splineChunkParent.transform);
            }
            splineChunkParent.SetActive(false);
            splineChunks.Add(splineChunkParent);
        }
        for(int j = 0; j < 1; j++){
            splineChunks[j].SetActive(true);
        }
    }

    public void RefineSplineLine(int cur, List<int> compareLines, BoxCollider[] platforms, ref SplineLineTest[] targets) {
        List<Vector3> pastDirs = new List<Vector3>();
        SplinePoint[] target = targets[cur].splineTargets;
        for(int i = 0; i < SmoothingRange; i++) {
            pastDirs.Add(new Vector3());
        }
        for(int i = 0; i < target.Length; i++){
            Vector3 dir = new Vector3();
            Vector3 tempDir = new Vector3();
            int count = 0;
            for (int j = 0; j < target.Length; j++) {
                foreach(var cl in compareLines){
                    tempDir.Set(target[i].pos.x, target[i].pos.y, target[i].pos.z);
                    tempDir -= targets[cl].splineTargets[j].pos;
                    float dist = Vector3.SqrMagnitude(tempDir);
                    if(dist < (1f + targets[cl].splineTargets[j].mass) * MergeDistance){
                        float weight = (1f/Mathf.Pow(dist,0.5f));
                        targets[cl].splineTargets[j].mass += weight;
                        target[i].mass += 0.5f * weight;
                        dir -= targets[cl].splineTargets[j].mass * tempDir;
                        count ++;
                    }
                }
            }
            for (int j = 0; j < platforms.Length; j++) {
                tempDir.Set(target[i].pos.x, target[i].pos.y, target[i].pos.z);
                tempDir -= platforms[j].transform.position;
                float dist = Vector3.SqrMagnitude(tempDir);
                if(dist < MergeDistance){
                    target[i].mass += (10f/dist);
                    dir -= 2f *tempDir;
                    count ++;
                }
            } 

            // if(i > 0){
            //     Vector3 dirToPrev = target[i-1] - target[i];
            //     dir += dirToPrev;
            // }

            Vector3 avgDir = new Vector3();

            pastDirs.RemoveAt(0);
            pastDirs.Add(dir);

            pastDirs.ForEach((Vector3 curDir) => {
                avgDir += curDir;
            });

            if((count == 0 || target[i].mass < .1f)){
                avgDir += new Vector3(0,-5f,0);
            }

            if(i+1 < target.Length){
                Vector3 nextPos = target[i].pos + (avgDir).normalized;
                Vector3 toNext = target[i+1].pos - target[i].pos;
                Vector3 toNew = target[i+1].pos - nextPos;
                target[i].pos = Vector3.Lerp(target[i].pos, target[i+1].pos - 5f * toNew.normalized, MergeStrength);
            } else {
                target[i].pos = Vector3.Lerp(target[i].pos, target[i].pos + (avgDir).normalized, MergeStrength);
            }
        }
    }

    void Update()
    {
        if(Iterations > 0){
            RefineSplineList();
            RefineSplineList();
            RefineSplineList();
            RefineSplineList();
            RefineSplineList();
            Iterations--;
        }
    }

    public void RefineSplineList() {
        // for(int it = 0; it < Iterations; it ++){
            foreach(var sc in splineChunks){
                SplineLineTest[] splineLines = sc.GetComponentsInChildren<SplineLineTest>();
                BoxCollider[] platforms = sc.GetComponentsInChildren<BoxCollider>();
                for(int i = 0; i < splineLines.Length; i++){
                    // reset mass 
                    for (int j = 0; j < splineLines[i].splineTargets.Length; j ++)
                    {
                        splineLines[i].splineTargets[j].mass = 0f;
                    }
                }
                for(int i = 0; i < splineLines.Length; i++){
                    if(splineLines[i].splineTargets.Length == 0) continue;
                    // compare with i + 2 and i - 2
                    List<int> compareLines = new List<int>();

                    for (int k = -10; k < 10; k ++){
                        if(k != 0 && i+k>=0 && i+k <= splineLines.Length - 1 && splineLines[i+k].splineTargets.Length == splineLines[i].splineTargets.Length){
                        compareLines.Add(i+k);
                        }
                    }

                    RefineSplineLine(i, compareLines, platforms, ref splineLines);
                    splineLines[i].InitSimple();
                }
            }
        }
    // }
}

#if UNITY_EDITOR

[CustomEditor(typeof(SplineSpawnerTest))]
public class SplineSpawnerTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SplineSpawnerTest myScript = (SplineSpawnerTest)target;
        if(GUILayout.Button("Create Spline List"))
        {
            myScript.CreateSplineList();
        }
        if(GUILayout.Button("Refine Spline List"))
        {
            myScript.RefineSplineList();
        }
        if(GUILayout.Button("Clear"))
        {
            myScript.Clear();
        }
        if(GUILayout.Button("Refresh"))
        {
            
        }

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