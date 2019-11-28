// MoveTo.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GrabbableObject : MonoBehaviour {
  public CCHand[] grabber = new CCHand[2]; 
  public Vector3 grabPoint;
  public Material mat;
  public virtual void onDown() {}
  public virtual void onHold() {}
  public virtual void onRelease() {}

  public void onDownBase() {
    onDown();
    mat.SetVector(
    "_WarpCenter", 
    new Vector4(grabPoint.x, grabPoint.y, grabPoint.z, 25));
    mat.SetFloat("_IgnoreGlobalStretch", 1);
  }

  Vector3 getGrabCenter() {
    Vector3 grabCenter = Vector3.zero;
    float count = 0;
    for(int i = 0 ; i < 2; i ++){
      if(!grabber[i]) continue;
      grabCenter += grabber[i].lastHandDif;
      count += 1f;
    }
    return grabCenter/count;
  }
  public void onHoldBase() {
    onHold();
    mat.SetVector("_WarpDir", 15f * getGrabCenter());
  }
  public void onReleaseBase() {
    onRelease();
    mat.SetVector("_WarpDir", Vector3.zero);
    mat.SetVector(
    "_WarpCenter", 
    new Vector4(100, 100, 100, 0));
  }
}
