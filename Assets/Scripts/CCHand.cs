using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Static Class to hand grab state
public class CCHand : MonoBehaviour
{
    public CCInput grabInput;
    GameObject grabbedAgent;
    Vector3 lastGrabbedAgentPos;
    Vector3 lastHandPos;
    List<Vector3> lastHandDifs = new List<Vector3>();
    int currentDifIdx = 0;
    Vector3 lastClosestPoint;
    public bool isGrabbing = false;
    Collider[] results = new Collider[5];
    public void LateUpdate(){

        switch(grabInput.GrabState){
            case GrabState.Down : {
                int length = Physics.OverlapSphereNonAlloc(transform.position, 0.4f, results);
                //get the closest one 
                float minDist = 1f;
                int minIdx = 0;
                for (int i = 0 ; i < length; i++){
                    Vector3 closestPoint = results[i].ClosestPoint(transform.position);
                    float dist = (closestPoint - transform.position).magnitude;
                    if(dist < minDist){
                        minIdx = i;
                        minDist = dist;
                        lastClosestPoint = closestPoint;
                    }
                }
                if (minDist < 0.2f){
                    grabbedAgent = results[minIdx].gameObject;
                    grabbedAgent.GetComponent<Animator>().SetTrigger("next");
                    grabbedAgent.GetComponentInChildren<SkinnedMeshRenderer>().material.SetVector(
                        "_WarpCenter", 
                        new Vector4(lastClosestPoint.x, lastClosestPoint.y, lastClosestPoint.z, 25));
                    // grabbedAgent.GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("_IgnoreGlobalStretch", 1);
                    lastGrabbedAgentPos = grabbedAgent.transform.position;
                    grabInput.TriggerHaptics();
                    isGrabbing = true;
                    // Time.timeScale = 0.3f;
                } else {
                    isGrabbing = false;
                }
                break;
            }
            case GrabState.Holding: {
                if(isGrabbing){
                    // check if player somehow got too far 
                    if(Vector3.Distance(CCPlayer.localPlayer.transform.position, grabbedAgent.transform.position) > 4f){
                        isGrabbing = false;
                        HandleRelease();
                    } else {
                        HandleGrabHold();
                    }
                }
                break;
            }
            case GrabState.Released: {
                if(isGrabbing){
                    isGrabbing = false;
                    HandleRelease();
                }
                break;
            }
        }

        lastHandPos = transform.position;
    }

    void HandleRelease() {
        grabbedAgent.GetComponent<Animator>().SetTrigger("next");
        grabbedAgent.GetComponentInChildren<SkinnedMeshRenderer>().material.SetVector("_WarpDir", Vector3.zero);
        grabbedAgent.GetComponentInChildren<SkinnedMeshRenderer>().material.SetVector(
        "_WarpCenter", 
        new Vector4(100, 100, 100, 0));
        //push you forward a bit in the last direction 
        StartCoroutine(forwardMomentum());          
    }

    void HandleGrabHold() {
        grabbedAgent.transform.position = lastGrabbedAgentPos;

        Vector3 lastHandDif = transform.position - lastHandPos;
        AddToList(lastHandDif);

        CCPlayer.localPlayer.transform.position -= lastHandDif.withY(0);

        grabbedAgent.GetComponentInChildren<SkinnedMeshRenderer>().material.SetVector("_WarpDir", 15f * lastHandDif);

        // float stretch = Shader.GetGlobalFloat("_GlobalStretch");
        // Shader.SetGlobalFloat("_GlobalStretch", Mathf.Min(stretch + 0.25f * Time.deltaTime, 3f));
    }

    void AddToList(Vector3 el) {
        lastHandDifs.Insert(0, el);
        if (lastHandDifs.Count > 3) {
            lastHandDifs.RemoveAt(3);
        }
    }

    IEnumerator forwardMomentum () {
        // bool shouldUnStretch = !CCPlayer.localPlayer.leftHand.isGrabbing && !CCPlayer.localPlayer.rightHand.isGrabbing;
        float stretch = Shader.GetGlobalFloat("_GlobalStretch");
        Vector3 avgHandDif = getAverageHandDif().withY(0);
        yield return this.xuTween((float rawT) => {
            float t = Easing.Cubic.In(rawT);
            // float t2 = Easing.Elastic.InOut(rawT);
            CCPlayer.localPlayer.transform.position -= (1f - t) * avgHandDif;
            // Time.timeScale = 0.3f + 0.7f * rawT;
            // if(shouldUnStretch){
            //     Shader.SetGlobalFloat("_GlobalStretch", (1f - t2) * stretch);
            // }
        }, 0.15f);
        // grabbedAgent.GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("_IgnoreGlobalStretch", 0);
        lastHandDifs.Clear();  
        yield return 0;
    }

    Vector3 getAverageHandDif() {
        Vector3 average = Vector3.zero;
        if(lastHandDifs.Count == 0) return average;

        for(int i = 0; i < lastHandDifs.Count; i++){
            average += lastHandDifs[i];
        }
        return average / lastHandDifs.Count;
    }
}