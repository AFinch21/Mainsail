using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTurn : MonoBehaviour
{
    public GameObject wheel;
    public bool xAxis;
    public bool zAxis;



    // Update is called once per frame
    void Update()
    {
        if (xAxis)
        {
            if (Input.GetKey(KeyCode.A))
            {
                wheel.transform.Rotate(0.0f, 0.0f, 0.8f, Space.Self);
            }
            if (Input.GetKey(KeyCode.D))
            {
                wheel.transform.Rotate(0.0f, 0.0f, -0.8f, Space.Self);
            }

            if (wheel.transform.localRotation.x < -0.1f && !Input.GetKey(KeyCode.A))
            {
                //Debug.Log("Wheel not straight");
                wheel.transform.Rotate(0.0f, 0.0f, -0.8f, Space.Self);
            }
            else if (wheel.transform.localRotation.x > 0.1f && !Input.GetKey(KeyCode.D))
            {
                //Debug.Log("Wheel not straight");
                wheel.transform.Rotate(0.0f, 0.0f, 0.8f, Space.Self);
            }
        }

        if (zAxis)
        {
            if (Input.GetKey(KeyCode.A))
            {
                wheel.transform.Rotate(-0.8f, 0.0f, 0.0f, Space.Self);
            }
            if (Input.GetKey(KeyCode.D))
            {
                wheel.transform.Rotate(0.8f, 0.0f, 0.0f, Space.Self);
            }
            
            Debug.Log(wheel.transform.localRotation.x);

            if (wheel.transform.localRotation.x < -0.1f && !Input.GetKey(KeyCode.A))
            {
                //Debug.Log("Wheel not straight");
                wheel.transform.Rotate(0.8f, 0.0f, 0.0f, Space.Self);
            }
            else if (wheel.transform.localRotation.x > 0.1f && !Input.GetKey(KeyCode.D))
            {
                //Debug.Log("Wheel not straight");
                wheel.transform.Rotate(-0.8f, 0.0f, 0.0f, Space.Self);
            }
        }
    }
}
