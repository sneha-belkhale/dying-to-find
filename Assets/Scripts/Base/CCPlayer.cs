using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Static Class to hand grab state
public class CCPlayer : MonoBehaviour
{
  public static CCPlayer main;
  public CCHand leftHand;
  public CCHand rightHand;
  public Transform head;

  public bool useGravity = false;

  public float globalScaleVal = 0;
  Vector3 pAcc;
  Vector3 pVelo;
  bool moving = false;
  private void Awake()
  {
    if (main == null) {
        main = this;
    }
    else {
        Destroy(main);
        main = this;
    }
    transform.position = transform.position.withY(1.8f);
    ResetAcceleration();
  }
  public void ResetAcceleration() {
      if (useGravity) {
        pVelo = Vector3.down - 80f * (leftHand.forwardMomentumVec + rightHand.forwardMomentumVec);
        pAcc = Vector3.up * -9.8f;
      }
  }
  private void Update()
  {
    //debugging
    Vector3 finalMove = Vector3.zero;
    finalMove.y += leftHand.grabInput.joystickInput.y;
    finalMove.y += rightHand.grabInput.joystickInput.y;

    // Movement
    if (isGrabbing) {
      moving = false;
    } else if (useGravity) {
        if(!moving) {
            ResetAcceleration();
            moving = true;
        }
        float armSpan = 0;
        armSpan = GetArmSpan();
        pVelo += GetArmSpanForce();
        pVelo.y = Mathf.Max(pVelo.y + pAcc.y * Time.deltaTime, -7.2f);
        // } 
        Vector3 damp = ((1f - 1.5f * Time.deltaTime) * Vector3.one).withY(armSpan);
        pVelo = Vector3.Scale(pVelo, damp);
        finalMove += Time.deltaTime * pVelo;
        transform.position += finalMove;
    }
  }

  float GetArmSpan(){
    float mag = Vector3.Magnitude(leftHand.transform.position.withY(0) - rightHand.transform.position.withY(0));
    return Mathf.Clamp(1f - 4f * Time.deltaTime * mag, 0f , 1f);
  }

  Vector3 GetArmSpanForce(){
    Vector3 headToHandDir = head.position - 0.5f * (leftHand.transform.position + rightHand.transform.position);
    return 0.08f * headToHandDir.withY(0);
  }

  private void OnDestroy()
  {
      main = null;
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

  Vector3 handDif(CCHand hand) {
    Vector3 grabPointDif = Vector3.zero;
    if(hand.isGrabbing){
      grabPointDif = hand.lastClosestPoint - hand.transform.position;
    }
    return grabPointDif;
  }
}