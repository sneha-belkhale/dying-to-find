using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineLine : MonoBehaviour
{
    public LineRenderer line;
    [SerializeField] GameObject viz;
    [SerializeField] int linePointsPerMeter = 4;
    public Vector3 endPos;
    public Vector3 attachPos;
    int cutOff = 52;
    float len;

    public Vector3 Init(int splineCount, float _len)
    {
        len = _len;
        float distance;
        Vector3[] splineTargets = generateSplineTargets(splineCount, out distance);
        setSplineLinePoints(splineTargets, distance);

        int endIdx = splineCount - 5 - (int)(5f / len) * Random.Range(0, 10);
        endPos = splineTargets[endIdx];
        attachPos = splineTargets[endIdx];
        return splineTargets[splineCount - cutOff];
    }

    public void InitSimple(Vector3[] splineTargets) {
        setSplineLinePoints(splineTargets, 4f);
    }

    Vector3[] generateSplineTargets(int splineCount, out float distance) { 
        Vector3[] splineTargets = new Vector3[splineCount];
        distance = 0;
        for (int i = 0; i < splineCount; i++)
        {
            if((i-cutOff) == 0) {
                splineTargets[i] = transform.position;
                continue;
            }
            float noise = Mathf.PerlinNoise(3f * (float)i / splineCount, 3f * transform.position.y);
            splineTargets[i] = len * ((i - cutOff) * transform.forward + 0.5f * noise * (i - cutOff) * transform.up) + transform.position;
            if (i > 0)
            {
                distance += Vector3.Distance(splineTargets[i], splineTargets[i - 1]);
            }
        }
        return splineTargets;
    }
    // public Vector3 InitWithPoints() {

    // }
    public void setSplineLinePoints(Vector3[] splineTargets, float distance) {
        line = GetComponent<LineRenderer>();
        line.widthMultiplier = 0.4f + Random.Range(-0.2f, 0.2f);

        int splineCount = splineTargets.Length;

        for (int i = 10; i < splineCount; i += 10) {
            GameObject go = new GameObject();
            go.transform.SetParent(transform);
            BoxCollider bc = go.AddComponent<BoxCollider>();
            Vector3 dif = splineTargets[i] - splineTargets[i-5];
            bc.transform.position = 0.5f * (splineTargets[i] + splineTargets[i-5]);
            bc.size = new Vector3(0.5f, 0.5f, 2f * dif.magnitude);
            bc.transform.rotation = Quaternion.LookRotation(dif.normalized);
            bc.gameObject.layer = LayerMask.NameToLayer("Ignore Collisions");
            GrabbableObject grab = go.AddComponent<GrabbableObject>();
            grab.mat = line.sharedMaterial;
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
    }

    // Update is called once per frame
    void Update()
    {

    }
}
