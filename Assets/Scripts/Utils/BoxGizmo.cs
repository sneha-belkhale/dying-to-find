using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BoxGizmo : MonoBehaviour
{
    public string label = "";

    void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        Handles.Label(transform.position, label, style);

        // Draw a yellow cube at the transform position
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
    }
}
