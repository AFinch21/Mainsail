using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindAngle : MonoBehaviour
{

    public GameObject hull;
    public GameObject sail;
    public GameObject mast;
    public float windDirection;
    public float sailAngle;
    private float hullAngle;
    private float sailHullDif;
    public float sailPower;
    public float boatTopSpeed;
    private bool sailOnCorrectSide;

    // Start is called before the first frame update
    void Start()
    {
        sailOnCorrectSide = false;
    }

    // Update is called once per frame
    void Update()
    {

        sailAngleCalcs();
        boatControls();

    }

    public void sailAngleCalcs()
    {
        sailAngle = sail.transform.eulerAngles.y;
        hullAngle = hull.transform.eulerAngles.y;
        sailHullDif = mast.transform.localRotation.z;

        //Get and correct sail angle (I just want to measure things in 180 or -180 degree hemispheres
        if (hullAngle > 180)
        {
            hullAngle = hullAngle - 360;
        }
        if (sailAngle > 180)
        {
            sailAngle = sailAngle - 360;
        }

        float sailWindAngle = sailAngle + windDirection;
        float hullWindAngle = hullAngle + windDirection;


        Debug.Log("Sail angle to wind is " + (sailWindAngle) + " and hull angle to wind is " + (windDirection + hullAngle) + ", difference between sail and hull is " + sailHullDif);
        //Debug.Log("Sail angle to wind is " + (sailWindAngle) + " and hull wind angle is " + (hullWindAngle));


        if ((hullWindAngle > 0) && (sailHullDif < 0.05))
        {
            Debug.Log("Sail is on wrong side! It should be on port");
            mast.transform.Rotate(0.0f, 0.0f, 0.8f, Space.Self);
        }
        else if ((hullWindAngle < 0) && (sailHullDif > -0.05))
        {
            Debug.Log("Sail is on wrong side! It should be on starboard");
            mast.transform.Rotate(0.0f, 0.0f, -0.8f, Space.Self);

        }

        if ((hullWindAngle > 140) || (hullWindAngle < -140))
        {
            Debug.Log("DEADZONE");
            sailPower = 0.1f;
        }
        else if(sailWindAngle >= 0)
        {
            if ((sailWindAngle > 0) && (sailWindAngle < 90))
            {
                sailPower = sailWindAngle / 90;
                Debug.Log(sailPower);
            }
            else if ((sailWindAngle > 90) && (sailWindAngle < 180))
            {
                sailPower = (180 - sailWindAngle) / 90;
                Debug.Log(sailPower);
            }
        }
        else if (sailWindAngle <= 0)
        {
            if ((sailWindAngle < 0) && (sailWindAngle > -90))
            {
                sailPower = sailWindAngle / -90;
                Debug.Log(sailPower);
            }
            else if ((sailWindAngle < -90) && (sailWindAngle > -180))
            {
                sailPower = (180 + sailWindAngle) / 90;
                Debug.Log(sailPower);
            }
        }
    }

    public void boatControls()
    {
        //control boom wihth Q and E
        if (Input.GetKey(KeyCode.E) && (mast.transform.localRotation.z > -0.65))
        {
            mast.transform.Rotate(0.0f, 0.0f, -0.8f, Space.Self);
        }
        if (Input.GetKey(KeyCode.Q) && (mast.transform.localRotation.z < 0.65))
        {
            mast.transform.Rotate(0.0f, 0.0f, 0.8f, Space.Self);
        }

        //control hull wihth A and D
        //if (Input.GetKey(KeyCode.D))
        //{
        //    hull.transform.Rotate(0.0f, 0.0f, 1.0f, Space.Self);
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    hull.transform.Rotate(0.0f, 0.0f, -1.0f, Space.Self);
        //}

        //gameObject.GetComponent<Rigidbody>().velocity = -transform.right * (boatTopSpeed * sailPower);
      
    }
}
