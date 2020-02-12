using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public struct SplinePoint {
    public Vector3 pos;
    public float mass;
}
public class SplineLineTest : MonoBehaviour
{
    public LineRenderer line;
    [SerializeField] GameObject viz;
    [SerializeField] int linePointsPerMeter = 4;
    public SplinePoint[] splineTargets;
    public GameObject[] splineTargetsViz;
    public Vector3 attachPos;
    int splineCount = 20;
    float len;

    public void Init(float _len)
    {
        len = _len;
        float distance;
        splineTargets = generateSplineTargets(out distance);
        setSplineLinePoints(distance);
        attachPos = splineTargets[splineTargets.Length - 5].pos;
    }

    public void InitSimple(SplinePoint[] _splineTargets = default(SplinePoint[])) {
        if(_splineTargets != null){
            splineTargets = _splineTargets;
        }
        float distance = 0;
        for (int i = 0; i < splineTargets.Length; i++)
        {
            if (i > 0)
            {
                distance += Vector3.Distance(splineTargets[i].pos, splineTargets[i - 1].pos);
            }
            splineTargetsViz[i].transform.position = splineTargets[i].pos;
            // splineTargetsViz[i].GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(splineTargets[i].mass/255f, 0, 0));
            

        }
        setSplineLinePoints(distance);
    }

    SplinePoint[] generateSplineTargets(out float distance) { 
        SplinePoint[] splineTargets = new SplinePoint[splineCount];
        splineTargetsViz = new GameObject[splineCount];
        distance = 0;

        for (int i = 0; i < splineCount; i++)
        {
            float noise2 = Mathf.PerlinNoise(3f * (float)i / splineCount - Time.deltaTime, 3f * transform.position.y);
            float noise = Mathf.PerlinNoise(3f * (float)i / splineCount, 3f * transform.position.y);
            splineTargets[i].pos = transform.position + len * i * transform.forward + 10f * noise * transform.up + 10f * noise2 * transform.right;
            splineTargetsViz[i] = Instantiate(viz, splineTargets[i].pos, Quaternion.identity, transform);
            if (i > 0)
            {
                distance += Vector3.Distance(splineTargets[i].pos, splineTargets[i - 1].pos);
            }
        }
        return splineTargets;
    }
    public void setSplineLinePoints(float distance) {
        line = GetComponent<LineRenderer>();
        line.widthMultiplier = 0.4f + Random.Range(-0.2f, 0.2f);

        int splineCount = splineTargets.Length;
        int numBoxColliders = Mathf.Max((int) (distance / 20f), 2);
        int step = splineCount / numBoxColliders;

        CatmullRom3 curve = new CatmullRom3();
        curve.Init(splineTargets.Select(el => el.pos).ToArray());

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
