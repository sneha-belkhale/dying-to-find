using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Static Class to hand grab state
public class CCHand : MonoBehaviour
{
    public CCGrabInput grabInput;
    GameObject grabbedAgent;
    Vector3 lastGrabbedAgentPos;
    Vector3 lastHandPos;
    Vector3 lastClosestPoint;
    bool isGrabbing = false;
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
                if (minDist < 1f){
                    grabbedAgent = results[minIdx].gameObject;
                    grabbedAgent.GetComponent<Animator>().SetTrigger("next");
                    grabbedAgent.GetComponentInChildren<SkinnedMeshRenderer>().material.SetVector(
                        "_WarpCenter", 
                        new Vector4(lastClosestPoint.x, lastClosestPoint.y, lastClosestPoint.z, 100));
                    lastGrabbedAgentPos = grabbedAgent.transform.position;
                    grabInput.TriggerHaptics();
                    isGrabbing = true;
                } else {
                    isGrabbing = false;
                }
                break;
            }
            case GrabState.Holding: {
                if(isGrabbing){
                    grabbedAgent.transform.position = lastGrabbedAgentPos;
                    Vector3 dif = transform.position - lastHandPos;
                    CCPlayer.localPlayer.transform.position -= dif.withY(0f);
                    grabbedAgent.GetComponentInChildren<SkinnedMeshRenderer>().material.SetVector("_WarpDir", 10f * dif);
                }
                break;
            }
            case GrabState.Released: {
                grabbedAgent.GetComponent<Animator>().SetTrigger("next");
                grabbedAgent.GetComponentInChildren<SkinnedMeshRenderer>().material.SetVector("_WarpDir", Vector3.zero);
                grabbedAgent.GetComponentInChildren<SkinnedMeshRenderer>().material.SetVector(
                        "_WarpCenter", 
                        new Vector4(100, 100, 100, 0));
                isGrabbing = false;
                break;
            }
        }

        lastHandPos = transform.position;
    }
}