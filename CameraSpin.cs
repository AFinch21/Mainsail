using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpin : MonoBehaviour
{

    public Transform vessel;
    Vector3 camPositionCurrent;
    Vector3 camPositionLevel;
    private bool menu;

    void Update()
    {
        if(menu)
        {
            transform.RotateAround(vessel.transform.position, Vector3.up, 20 * Time.deltaTime);
        }
        camPositionCurrent = gameObject.transform.position;
        Debug.Log(camPositionCurrent);
    }

    void moveToLevel()
    {

    }


}
