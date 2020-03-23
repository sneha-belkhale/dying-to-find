using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputState {
    Down,
    Holding,
    Released,
    Up,
}

public class CCInput : MonoBehaviour
{
public string hand;
private OVRInput.Controller ovrHand;
public InputState grabState;
public InputState triggerState;
public Vector2 joystickInput;
    void Start(){
        if(hand == "left"){
            ovrHand = OVRInput.Controller.LTouch;
        } else {
            ovrHand = OVRInput.Controller.RTouch;
        }
        triggerState = InputState.Up;
        grabState = InputState.Up;
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
        HandleOVRStateForButton(OVRInput.Button.PrimaryIndexTrigger, ref triggerState);
        HandleOVRStateForButton(OVRInput.Button.PrimaryHandTrigger, ref grabState);
        // handle joystick movement
        joystickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, ovrHand);
    }

    void HandleOVRStateForButton(OVRInput.Button button, ref InputState state)
    {
        switch (state) {
            case InputState.Down: {
                if(OVRInput.Get(button, ovrHand)){
                    state = InputState.Holding;
                }
                break;
            }
            case InputState.Holding: {
                if(!OVRInput.Get(button, ovrHand)){
                    state = InputState.Released;
                }
                break;
            }
            case InputState.Released: {
                if(OVRInput.GetDown(button, ovrHand)){
                    state = InputState.Down;
                } else {
                    state = InputState.Up;
                }
                break;
            }
            case InputState.Up: {
                if(OVRInput.GetDown(button, ovrHand)){
                    state = InputState.Down;
                }
                break;
            }
        }
    }
}