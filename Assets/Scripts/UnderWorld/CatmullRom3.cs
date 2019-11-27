using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//5
public class CatmullRom3 : MonoBehaviour
{
    Vector3[] _points;
    Vector3 p0, p1, t0, t1, c0, c1, c2, c3;
    public void Init(Vector3[] points){
        _points = points;
    }
    public Vector3 getPointAt(float t){
        if(t < 0.0001){
            return _points[0];
        }
        int upper = Mathf.CeilToInt(t * (_points.Length - 1));
        float weight = (t * (_points.Length - 1) - upper + 1 );
        p0 = _points[upper-1];
        p1 = _points[upper];
        t0 = (upper - 2 < 0) ? Vector3.one :  p1 - _points[upper-2];
        t1 = (upper + 1 >= _points.Length) ? Vector3.one :  _points[upper+1] - p0;
        return Calc(p0, p1, 0.5f * t0, 0.5f * t1, weight);
    }
    Vector3 Calc(Vector3 x0, Vector3 x1, Vector3 t0, Vector3 t1, float t){
        float t2 = t * t;
        float t3 = t2 * t;
        c0 = x0;
		c1 = t0;
		c2 = - 3 * x0 + 3 * x1 - 2 * t0 - t1;
		c3 = 2 * x0 - 2 * x1 + t0 + t1;
        return c0 + c1 * t + c2 * t2 + c3 * t3;
    }
}
