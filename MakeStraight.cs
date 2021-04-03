using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeStraight : MonoBehaviour
{
    public Transform Cam;
    public Transform camTarget;

    // Update is called once per frame
    void Update()
    {
        Cam.rotation.Set(90.0f, 0.0f, 90.0f, 0.0f);

    }
}
