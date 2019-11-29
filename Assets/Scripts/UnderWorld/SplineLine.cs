using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineLine : MonoBehaviour
{
    LineRenderer line;
    [SerializeField] GameObject viz;
    [SerializeField] int linePointsPerMeter = 4;
    public Vector3 endPos;
    public Vector3 Init()
    {

        int splineCount = 100;
        Vector3[] splineTargets = new Vector3[splineCount];
        float distance = 0;
        float scale = 1;
        int cutOff = 52;
        // splineTargets[] = transform.position;
        for (int i = 0; i < splineCount; i++)
        {
            if((i-cutOff) == 0) {
                splineTargets[i] = transform.position;
                continue;
            }
            float noise = Mathf.PerlinNoise(3f * (float)i / splineCount, 3f * transform.position.y);
            // float noise = 1f;
            splineTargets[i] = scale * (i - cutOff) * transform.forward + 0.5f * noise * (i - cutOff) * transform.up + transform.position;
            if (i > 0)
            {
                distance += Vector3.Distance(splineTargets[i], splineTargets[i - 1]);
            }
        }
        line = GetComponent<LineRenderer>();

        for (int i = 10; i < splineCount; i += 10){
            GameObject go = new GameObject();
            go.transform.SetParent(transform);
            BoxCollider bc = go.AddComponent<BoxCollider>();
            Vector3 dif = splineTargets[i] - splineTargets[i-5];
            bc.transform.position = 0.5f * (splineTargets[i] + splineTargets[i-5]);
            bc.size = new Vector3(0.5f, 0.5f, 2f * dif.magnitude);
            bc.transform.rotation = Quaternion.LookRotation(dif.normalized);
            GrabbableObject grab = go.AddComponent<GrabbableObject>();
            grab.mat = line.sharedMaterial;
        // Instantiate(viz, splineTargets[i], Quaternion.identity);
        }

        CatmullRom3 curve = new CatmullRom3();
        curve.Init(splineTargets);

        line.positionCount = (int)(linePointsPerMeter * distance);
        Vector3[] splinePoints = new Vector3[line.positionCount];
        for (int i = 0; i < line.positionCount; i++)
        {
            float t = (float)i / line.positionCount;
            splinePoints[i] = curve.getPointAt(t);
            // Instantiate(viz, splinePoints[i], Quaternion.identity);
        }
        line.SetPositions(splinePoints);
        endPos = splineTargets[splineCount - 10];
        return splineTargets[splineCount - cutOff];
    }

    // Update is called once per frame
    void Update()
    {

    }
}
