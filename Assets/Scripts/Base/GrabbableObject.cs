// MoveTo.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GrabbableObject : MonoBehaviour {
  public CCHand grabber; 
  public Vector3 grabPoint;
  public bool isGrabbed;
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
  public void onHoldBase() {
    onHold();
    mat.SetVector("_WarpDir", 15f * grabber.lastHandDif);
  }
  public void onReleaseBase() {
    onRelease();
    mat.SetVector("_WarpDir", Vector3.zero);
    mat.SetVector(
    "_WarpCenter", 
    new Vector4(100, 100, 100, 0));
  }
}
