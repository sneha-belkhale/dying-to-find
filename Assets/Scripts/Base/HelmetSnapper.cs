using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmetSnapper: MonoBehaviour {
    Rigidbody rb;
    public Transform snapTarget;
    int handIndex = 0;
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if(Vector3.SqrMagnitude(transform.position - snapTarget.position) < 1f)
        {
            rb.isKinematic = true;
            transform.position = snapTarget.position;
        }
    }
}