using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HelmetSnapper: MonoBehaviour {
    Rigidbody rb;
    [SerializeField] public Transform snapTarget;
    [SerializeField] Transform targetRoot;
    bool snapped = false;
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if(Vector3.SqrMagnitude(transform.position - snapTarget.position) < 0.5f && !snapped)
        {
            rb.isKinematic = true;
            snapped = true;
            transform.position = snapTarget.position;
            StartCoroutine(SuckUp());
            UnderWorldLevelCode.instance.NumPlatformsCompleted ++;
        }
    }
    IEnumerator SuckUp()
    {
        // scale down the character while maintaining head position, send three pulses out
        float initialHeight = snapTarget.position.y - targetRoot.position.y;
        Vector3 initialScale = targetRoot.localScale;
        ShaderEnvProps.instance.RecordGlobalPulse(snapTarget.position);
        this.xuTween((float t) => {
            float scaledHeight = t * initialHeight;
            targetRoot.localScale = initialScale.withY((1f - t) * initialScale.y);
            targetRoot.localPosition = scaledHeight * Vector3.up;
        }, 1.3f);

        yield return new WaitForSeconds(0.4f);
        ShaderEnvProps.instance.RecordGlobalPulse(snapTarget.position);
        yield return new WaitForSeconds(0.6f);
        ShaderEnvProps.instance.RecordGlobalPulse(snapTarget.position);
        yield return new WaitForSeconds(0.3f);
        
        rb.isKinematic = false;
        targetRoot.gameObject.SetActive(false);

        ShaderEnvProps.instance.RecordGlobalPulse(snapTarget.position);
        yield return 0;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(HelmetSnapper))]
public class HelmetSnapperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Shader.SetGlobalFloat("_GameTime", Time.fixedTime);

        DrawDefaultInspector();
        HelmetSnapper myScript = (HelmetSnapper)target;
        if(GUILayout.Button("Test Pulse"))
        {
            ShaderEnvProps.instance.RecordGlobalPulse(myScript.snapTarget.position);
        }
    }
}
#endif