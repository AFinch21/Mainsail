using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform CameraPosition;
    public Transform CameraTarget;

    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 desiredPostion = CameraPosition.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp (transform.position, desiredPostion, smoothSpeed * Time.deltaTime);



        transform.position = smoothedPosition;

        transform.LookAt(CameraTarget);
        
    }
}
