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
  }

  private void Update()
  {
#if UNITY_EDITOR
        // Shader.SetGlobalFloat("_GlobalStretch", globalScaleVal);
#endif
    //debugging 
    Vector3 curPos = transform.position;
    curPos.y += leftHand.grabInput.joystickInput.y;
    curPos.y += rightHand.grabInput.joystickInput.y;
    transform.position = curPos;
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