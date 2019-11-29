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
    List<GameObject> splineChunks;
    float count = 0;
    void Start()
    {
        ss.SetActive(false);
        splineChunks = new List<GameObject>();
        for(int j = 0; j < 30; j++){
            // Instantiate
            GameObject splineChunkParent = new GameObject();
            for(int k = 0; k < 30; k++){
                AddObject(splineChunkParent.transform);
                // if(k==15){
                //     Debug.Log("center maps " + j + " to : " + lastPosition.y + " " + lastPosition.y/j);
                // }
            }
            splineChunkParent.SetActive(false);
            splineChunks.Add(splineChunkParent);
        }
        for(int j = 0; j < 3; j++){
            splineChunks[j].SetActive(true);
        }
    }

    // Update is called once per frame
    public void AddObject(Transform parent)
    {
        // float r = 2f * random * Mathf.PerlinNoise(count, 4f * Mathf.Sin(count/10f)) - random;
        float r = Random.Range(-20f, 20f);
        SplineLine sl = Instantiate(ss, lastPosition, Quaternion.Euler(0f, (360f / 4f) * (count%4) + r, 0f), parent).GetComponent<SplineLine>();
        sl.gameObject.SetActive(true);
        lastPosition = sl.Init();
        count++;
        //every 8 lines, we add a platform, 
        // if(count%5 == 4){
        //     Instantiate(platform, sl.endPos, Quaternion.identity);
        //     //instantiate 3 connecting lines

        // }
    }

    public int curChunkPos = 1;
    public int lastChunkPos = 1;
    public void Update() {
        curChunkPos = Mathf.RoundToInt(CCPlayer.localPlayer.transform.position.y / -30f);
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

    // public void Clear() {
    //     foreach(var spline in splineList) {
    //         Destroy(spline.gameObject);
    //     }
    //     splineList.Clear();
    //     lastPosition = Vector3.zero;
    //     count = 0;
    // }
}

// #if UNITY_EDITOR

// [CustomEditor(typeof(SplineSpawner))]
// public class SplineSpawnerEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();
//         SplineSpawner myScript = (SplineSpawner)target;
//         if(GUILayout.Button("Build Object"))
//         {
//             myScript.AddObject();
//         }
//         if(GUILayout.Button("Clear"))
//         {
//             myScript.Clear();
//         }
//     }
// }

// #endif