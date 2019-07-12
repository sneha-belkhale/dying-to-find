using UnityEngine;
using System.Collections;

public class ControllsRotator : MonoBehaviour
{
    private Quaternion targetRotation;
    public float rotationSpeed = 0.80f;

    void Start()
    {
        targetRotation = transform.rotation;
    }
    void Update()
    {
        float angle = Mathf.Abs(transform.eulerAngles.y - Camera.main.transform.eulerAngles.y);
        if(angle > 90f)
        {
            targetRotation = Quaternion.Euler(0.0f, Camera.main.transform.eulerAngles.y, 0.0f);
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
