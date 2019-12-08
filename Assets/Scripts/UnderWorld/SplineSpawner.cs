using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class SplineSpawner : MonoBehaviour
{
    public GameObject ss;
    public GameObject platform;
    
    [Header("SPAWN PARAMS")]
    [SerializeField] float random = 360f;

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
        float r = Random.Range(-20f, 20f);
        float rz = Random.Range(1f, 6f);
        SplineLine sl = Instantiate(ss, lastPosition.withY(lastPosition.y - rz), Quaternion.Euler(0f, (360f / 4f) * (count%4) + r, 0f), parent).GetComponent<SplineLine>();
        sl.gameObject.SetActive(true);
        sl.gameObject.name = "Spline";
        lastPosition = sl.Init();
        count++;
        //every 8 lines, we add a platform, 
        if(count%5 == 4 && rz > 4f){
            GameObject pl = Instantiate(platform, sl.endPos.withY(sl.endPos.y + 2f), Quaternion.identity, sl.gameObject.transform);
            pl.SetActive(true);
        }
    }

    private int curChunkPos = 1;
    private int lastChunkPos = 1;
    public Transform sphere;
    public float w;
    public void Update() {
        curChunkPos = Mathf.RoundToInt(CCPlayer.main.transform.position.y / -30f);
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