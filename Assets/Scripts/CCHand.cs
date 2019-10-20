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
                    float dist = (results[i].ClosestPoint(transform.position) - transform.position).magnitude;
                    if(dist < minDist){
                        minIdx = i;
                        minDist = dist;
                    }
                }
                if (minDist < 1f){
                    grabbedAgent = results[minIdx].gameObject;
                    grabbedAgent.GetComponent<Animator>().SetTrigger("next");
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
                }
                break;
            }
            case GrabState.Released: {
                grabbedAgent.GetComponent<Animator>().SetTrigger("next");
                isGrabbing = false;
                break;
            }
        }

        lastHandPos = transform.position;
    }
}