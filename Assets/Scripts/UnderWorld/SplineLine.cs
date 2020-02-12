﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineLine : MonoBehaviour
{
    public LineRenderer line;
    [SerializeField] GameObject viz;
    [SerializeField] int linePointsPerMeter = 4;
    public Vector3[] splineTargets;
    public Vector3 attachPos;
    int splineCount = 20;

    int cutOff = 20/2;
    float len;

    public Vector3 Init(float _len)
    {
        len = _len;
        float distance;
        splineTargets = generateSplineTargets(out distance);
        setSplineLinePoints(distance);
        attachPos = splineTargets[splineCount - (int)(splineCount/10f) - Random.Range(0,4)];
        return splineTargets[splineCount - cutOff];
    }

    public void InitSimple(Vector3[] _splineTargets = default(Vector3[])) {
        if(_splineTargets != null){
            splineTargets = _splineTargets;
        }
        float distance = 0;
        for (int i = 0; i < splineTargets.Length; i++)
        {
            if (i > 0)
            {
                distance += Vector3.Distance(splineTargets[i], splineTargets[i - 1]);
            }
        }
        setSplineLinePoints(distance);
    }

    Vector3[] generateSplineTargets(out float distance) { 
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
    public void setSplineLinePoints(float distance) {
        line = GetComponent<LineRenderer>();
        line.widthMultiplier = 0.4f + Random.Range(-0.2f, 0.2f);

        int splineCount = splineTargets.Length;
        int numBoxColliders = Mathf.Max((int) (distance / 20f), 2);
        int step = splineCount / numBoxColliders;
        // for (int i = 0; i < numBoxColliders - 1; i ++) {
        //     GameObject go = new GameObject();
        //     go.transform.SetParent(transform);
        //     BoxCollider bc = go.AddComponent<BoxCollider>();
        //     int cur = step * i;
        //     int next = step * (i+1);
        //     Vector3 dif = splineTargets[next] - splineTargets[cur];
        //     bc.transform.position = 0.5f * (splineTargets[cur] + splineTargets[next]);
        //     bc.size = new Vector3(0.5f, 0.5f, dif.magnitude);
        //     bc.transform.rotation = Quaternion.LookRotation(dif.normalized);
        //     bc.gameObject.layer = LayerMask.NameToLayer("Ignore Collisions");
        //     GrabbableObject grab = go.AddComponent<GrabbableObject>();
        //     grab.mat = line.sharedMaterial;
        // }

        CatmullRom3 curve = new CatmullRom3();
        curve.Init(splineTargets);

        line.positionCount = (int)(linePointsPerMeter * distance);
        Vector3[] splinePoints = new Vector3[line.positionCount];
        for (int i = 0; i < line.positionCount; i++)
        {
            float t = (float)i / line.positionCount;
            splinePoints[i] = curve.getPointAt(t);
        }
        line.SetPositions(splinePoints);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
