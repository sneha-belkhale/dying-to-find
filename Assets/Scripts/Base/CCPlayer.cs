using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Static Class to hand grab state
public class CCPlayer : MonoBehaviour
{
  public static CCPlayer localPlayer;
  public CCHand leftHand;
  public CCHand rightHand;
  public Transform head;

  public bool antiGravity = false;

  public float globalScaleVal = 0;
  Vector3 pAcc;
  Vector3 pVelo;
  bool falling = false;
  private void Awake()
  {
    if (localPlayer == null) {
        localPlayer = this;
    }
    else {
        Destroy(localPlayer);
        localPlayer = this;
    }
    transform.position = transform.position.withY(1.8f);
    ResetAcceleration();
  }
  void ResetAcceleration() {
      if (antiGravity) {
        pVelo = Vector3.down - 80f * (leftHand.forwardMomentumVec + rightHand.forwardMomentumVec);
        pAcc = Vector3.up * -9.8f;
      } else {
        pVelo = -50f * (leftHand.forwardMomentumVec + rightHand.forwardMomentumVec);
        pAcc = Vector3.zero;
      }
  }
  private void Update()
  {
    //debugging 
    Vector3 curPos = transform.position;
    curPos.y += leftHand.grabInput.joystickInput.y;
    curPos.y += rightHand.grabInput.joystickInput.y;
    transform.position = curPos;

    Vector3 pos = CCPlayer.localPlayer.transform.position;

    // Movement
    if(isGrabbing){
      falling = false;
    } else {
        if(!falling) {
            // reset velocity
            ResetAcceleration();
            falling = true;
        }
        pVelo.y = Mathf.Max(pVelo.y + pAcc.y * Time.deltaTime, -4.5f);
        Vector3 damp = ((1f - 1.5f * Time.deltaTime) * Vector3.one).withY(1f);
        pVelo = Vector3.Scale(pVelo, damp);
        pos += Time.deltaTime * pVelo;
    }

    CCPlayer.localPlayer.transform.position = pos;
  }

  private void OnDestroy()
  {
      localPlayer = null;
      Shader.SetGlobalFloat("_GlobalStretch", 0);
  }

  public void Teleport(Transform t){
    transform.SetPositionAndRotation(t.position, t.rotation);
  }
  public bool isGrabbing {
    get {
      return (leftHand.isGrabbing || rightHand.isGrabbing);
    }
  }
}