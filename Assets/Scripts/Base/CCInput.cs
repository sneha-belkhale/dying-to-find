using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GrabState {
Down,
Holding,
Released,
}

public class CCInput : MonoBehaviour
{
public string hand;
private OVRInput.Controller ovrHand;
public GrabState GrabState;
public Vector2 joystickInput;
    void Start(){
        if(hand == "left"){
            ovrHand = OVRInput.Controller.LTouch;
        } else {
            ovrHand = OVRInput.Controller.RTouch;
        }
    }

    public void TriggerHaptics() {
        OVRInput.SetControllerVibration(0.3f, 0.3f, ovrHand);
        StopCoroutine("StopVibration");
        StartCoroutine("StopVibration");
    }

    IEnumerator StopVibration() {
       yield return new WaitForSeconds(0.2f);
       OVRInput.SetControllerVibration(0f, 0f, ovrHand);
       yield return 0;
    }
    void Update(){
        switch (GrabState) {
            case GrabState.Down: {
                if(OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, ovrHand)){
                    GrabState = GrabState.Holding;
                }
                break;
            }
            case GrabState.Holding: {
                if(!OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, ovrHand)){
                    GrabState = GrabState.Released;
                }
                break;
            }
            case GrabState.Released: {
                if(OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, ovrHand)){
                    GrabState = GrabState.Down;
                }
                break;
            }
        }

        // handle joystick movement
        joystickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, ovrHand);
    }
}