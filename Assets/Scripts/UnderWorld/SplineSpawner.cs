using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class SplineSpawner : MonoBehaviour
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

        float r = Random.Range(-20f, 20f);
        float rz = spacing * Random.Range(0f, 1f);
        SplineLine sl = Instantiate(ss, lastPosition.withY(lastPosition.y - rz), Quaternion.identity, parent).GetComponent<SplineLine>();
        sl.gameObject.SetActive(true);
        sl.gameObject.name = "Spline";
        lastPosition = sl.Init(length);
        count++;
  
        // //every 8 lines, we add a platform, 
        // if (count%5 == 4 && rz > 4f) {
        //     GameObject pl = Instantiate(platform, sl.endPos.withY(sl.endPos.y + 2f * spacing), Quaternion.identity, sl.gameObject.transform);
        //     SplineLine sls = Instantiate(ss, sl.endPos, Quaternion.identity, pl.transform).GetComponent<SplineLine>();
        //     Vector3[] splineTargets = new Vector3[3];
        //     splineTargets[0] = sl.attachPos;
        //     splineTargets[1] = 0.5f * (sl.attachPos + pl.transform.position) + 3f * length * sl.transform.forward;
        //     splineTargets[2] = pl.transform.position;
        //     sls.InitSimple(splineTargets);
        //     sls.line.widthMultiplier *= 0.5f;
        //     sls.gameObject.SetActive(true);
        //     pl.SetActive(true);
        // }
        // if(count %30 == 0){
        //     Debug.Log(sl.endPos.y / Mathf.Floor(count/30));
        // }

                //every 8 lines, we add a platform, 
        if (count%5 == 4) {
            GameObject pl = Instantiate(platform, sl.attachPos.withY(sl.attachPos.y + 2f * spacing), Quaternion.identity, sl.gameObject.transform);
            pl.SetActive(true);
        }
    }

    private int curChunkPos = 1;
    private int lastChunkPos = 1;
    public Transform sphere;
    public float w;
    public void Update() {
        curChunkPos = Mathf.RoundToInt((CCPlayer.main.transform.position.y+30f) / -181f);
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
        for(int j = 0; j < 3; j++){
            // Instantiate
            GameObject splineChunkParent = new GameObject("SplineChunk");
            splineChunkParent.transform.SetParent(transform);
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

    public void RefineSplineLine(List<Vector3[]> compareLines, BoxCollider[] platforms, ref Vector3[] target) {
        List<Vector3> pastDirs = new List<Vector3>();
        for(int i = 0; i < SmoothingRange; i++) {
            pastDirs.Add(new Vector3());
        }
        for(int i = 0; i < target.Length; i++){
            // int start = Mathf.Max(i-5,0);
            // int end = Mathf.Min(i+5,target.Length-1);
            Vector3 dir = new Vector3();
            Vector3 tempDir = new Vector3();
            for (int j = 0; j < target.Length; j++) {
                foreach(var cl in compareLines){
                    tempDir.Set(target[i].x, target[i].y, target[i].z);
                    tempDir -= cl[j];
                    float dist = Vector3.SqrMagnitude(tempDir);
                    if(dist < 0.1f * MergeDistance){
                        dir -= (1f/dist) * tempDir;
                    }
                }
            }
            // for (int j = 0; j < platforms.Length; j++) {
            //     tempDir.Set(target[i].x, target[i].y, target[i].z);
            //     tempDir -= platforms[j].transform.position;
            //     float dist = Vector3.SqrMagnitude(tempDir);
            //     if(dist < 2f * MergeDistance){
            //         dir -=  (10f/dist) * tempDir;
            //     }
            // } 

            Vector3 avgDir = new Vector3();

            pastDirs.RemoveAt(0);
            pastDirs.Add(dir);

            pastDirs.ForEach((Vector3 curDir) => {
                avgDir += curDir;
            });
            target[i] = Vector3.Lerp(target[i], target[i] + (avgDir).normalized , Mathf.Min(count, MergeStrength));

        }
    }

    public void RefineSplineList() {
        for(int it = 0; it < Iterations; it ++){
        foreach(var sc in splineChunks){
            SplineLine[] splineLines = sc.GetComponentsInChildren<SplineLine>();
            BoxCollider[] platforms = sc.GetComponentsInChildren<BoxCollider>();
            for(int i = 0; i < splineLines.Length; i++){
                if(splineLines[i].splineTargets.Length == 0) continue;
                // compare with i + 2 and i - 2
                List<Vector3[]> compareLines = new List<Vector3[]>();

                for (int k = -3; k < 4; k ++){
                    if(k != 0 && i+k>=0 && i+k <= splineLines.Length - 1 && splineLines[i+k].splineTargets.Length == splineLines[i].splineTargets.Length){
                    compareLines.Add(splineLines[i+k].splineTargets);
                    }
                }

                RefineSplineLine(compareLines, platforms, ref splineLines[i].splineTargets);
                splineLines[i].InitSimple();
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