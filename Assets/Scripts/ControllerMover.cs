using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerMover : MonoBehaviour
{
  public float mouseSpeed = 2.0f;
  private float yaw = 0.0f;
  private float pitch = 0.0f;
  private float scale = 1.5f;

  void Start()
  {
    Cursor.lockState = CursorLockMode.Locked;
  }

  void Update()
  {
    bool isConnected = OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote);

    if (isConnected)
    {
      transform.rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);
    }
    else
    {
      yaw += mouseSpeed * Input.GetAxis("Mouse X");
      pitch -= mouseSpeed * Input.GetAxis("Mouse Y");
      transform.localEulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
  }
}
