using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFaceCamera : MonoBehaviour
{
    public GameObject cam;
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam.transform);
    }

}
