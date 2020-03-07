using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryGrabbableObject : GrabbableObject {
    Rigidbody rb;
    Vector3 positionOffset;
    Quaternion rotationOffset;
    int handIndex = 0;
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        mat = GetComponent<MeshRenderer>().material;    
    }
    public override void onDown()
    {
        rb.isKinematic = true;
        handIndex = (grabber[0] != null) ? 0 : 1;
        // set the initial offset 
        positionOffset = grabber[handIndex].transform.position - transform.position;
        //Offset rotation
        rotationOffset = Quaternion.Inverse(grabber[handIndex].transform.localRotation * transform.localRotation);
    }
    public override void onHold()
    {
        // move with the initial offset
        var targetPos = grabber[handIndex].transform.position - positionOffset;
        var targetRot = grabber[handIndex].transform.localRotation * rotationOffset;
 
        transform.position = RotatePointAroundPivot(targetPos, grabber[handIndex].transform.position, targetRot);
        transform.localRotation = targetRot;
    }
    public override void onRelease()
    {
        rb.isKinematic = false;
    }
    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        //Get a direction from the pivot to the point
        Vector3 dir = point - pivot;
        //Rotate vector around pivot
        dir = rotation * dir; 
        //Calc the rotated vector
        point = dir + pivot; 
        //Return calculated vector
        return point; 
    }
}