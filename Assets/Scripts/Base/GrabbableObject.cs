// MoveTo.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GrabbableObject : MonoBehaviour {
  public CCHand grabber; 
  public Vector3 grabPoint;
  public bool isGrabbed;
  Material matz;
  protected virtual void Start() {
    SkinnedMeshRenderer r = GetComponentInChildren<SkinnedMeshRenderer>();
    if(r != null ){
      matz = r.material;
    } else {
      matz = GetComponentInChildren<Renderer>().material;
    }
  }
  void Update() {
      handleGrab();
  }
  public virtual void onDown() {}
  public virtual void onHold() {}
  public virtual void onRelease() {}

  void onDownBase() {
    onDown();
    matz.SetVector(
    "_WarpCenter", 
    new Vector4(grabPoint.x, grabPoint.y, grabPoint.z, 25));
    matz.SetFloat("_IgnoreGlobalStretch", 1);
  }
  void onHoldBase() {
    onHold();
    matz.SetVector("_WarpDir", 15f * grabber.lastHandDif);
  }
  void onReleaseBase() {
    onRelease();
    matz.SetVector("_WarpDir", Vector3.zero);
    matz.SetVector(
    "_WarpCenter", 
    new Vector4(100, 100, 100, 0));
  }

  private void handleGrab() {
    if(grabber){
      if(!isGrabbed) {
        onDownBase();
        isGrabbed = true;
      } else {
        onHoldBase();
      }
    } else {
      if(isGrabbed){
        onReleaseBase();
        isGrabbed = false;
      }
    }
  }
}
