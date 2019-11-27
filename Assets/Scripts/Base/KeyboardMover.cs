using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMover : MonoBehaviour
{
    public float mouseSpeed = 20.0f;
    public float moveSpeed = 2.0f;
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void move(Vector3 direction)
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }

    void Update()
    {
        float boost = 1.0f;

        if (Input.GetKey(KeyCode.B))
        {
          boost = 20.0f;
        }

        if (Input.GetKey(KeyCode.R))
            yaw -= mouseSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E))
            yaw += mouseSpeed * Time.deltaTime;

        //pitch -= mouseSpeed * Input.GetAxis("Mouse Y");
        transform.localEulerAngles = new Vector3(0.0f, yaw, 0.0f);

        if (Input.GetKey(KeyCode.W))
            move(transform.forward);

        if (Input.GetKey(KeyCode.S))
            move(-transform.forward);

        if (Input.GetKey(KeyCode.D))
            move(transform.right);

        if (Input.GetKey(KeyCode.A))
            move(-transform.right);

        if (Input.GetKey(KeyCode.DownArrow))
            move(-transform.up * boost);
        if (Input.GetKey(KeyCode.UpArrow))
            move(transform.up * boost);

    }
}
