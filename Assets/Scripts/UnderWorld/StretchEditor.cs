using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class StretchEditor : MonoBehaviour
{
    public GameObject stretchy;
    public Transform target;
    public float w;
}

#if UNITY_EDITOR

[CustomEditor(typeof(StretchEditor))]
public class StretchEditorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
    }

     void OnEnable() { EditorApplication.update += Update; }
     void OnDisable() { EditorApplication.update -= Update; }
 
     void Update()
     {
        StretchEditor myScript = (StretchEditor)target;

        Vector3 closestPoint = myScript.stretchy.GetComponent<SphereCollider>().ClosestPoint(myScript.target.position);
        Vector3 dir = myScript.target.position - closestPoint;
        myScript.stretchy.GetComponent<Renderer>().sharedMaterial.SetVector("_WarpDir", dir);
        myScript.stretchy.GetComponent<Renderer>().sharedMaterial.SetVector(
                        "_WarpCenter",
                        new Vector4(closestPoint.x, closestPoint.y, closestPoint.z, myScript.w));
     }
}

#endif