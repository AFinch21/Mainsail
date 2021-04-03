using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class mainMenu : MonoBehaviour
{
    public Transform vessel;
    public Transform ModeSelectCameraPoint;
    public Transform MapSelectCamera;
    private Transform camPosition;
    public GameObject startingPosition;
    public GameObject menuCam;
    public float camMoveTime = 1.0f;
    public float startTime;
    
    public bool menu;
    private bool camPanMode;
    private bool camPanMap;
    private bool camPanModeBack;
    private float fraComplete = 0.0f;
    public float rate = 0.5f;
 

    private void Start()
    {
        menu = true;
    }

    void Update()
    {
        if (menu == true)
        {
            rotateCamera();
        }


        if (camPanMode)
        {
            zoomToModeSelect();
        }

        if (camPanMap)
        {
            zoomToMapSelect();
        }


        if (camPanModeBack)
        {
            resetCamera();
        }
        camPosition = menuCam.transform;
        //Debug.Log("Movement Faction: " + fraComplete);

    }


    // Start is called before the first frame update
    public void modeSelect()
    {
        fraComplete = 0.0f;
        camPanMode = true;
    }

    public void mapSelect()
    {
        fraComplete = 0.0f;
        camPanMap = true;
    }

    public void modeBack()
    {
        fraComplete = 0.0f;
        camPanModeBack = true;
    }

    public void rotateCamera()
    {
        menuCam.transform.RotateAround(vessel.transform.position, Vector3.up, rate);
    }

    public void zoomToModeSelect()
    {
        fraComplete = fraComplete + 0.01f;

        Vector3 desiredPosition = ModeSelectCameraPoint.transform.position;
        Vector3 camPositionCurrent = camPosition.transform.position;

        menuCam.transform.position = Vector3.Slerp(camPositionCurrent, desiredPosition, fraComplete);

        menuCam.transform.rotation = Quaternion.Slerp(camPosition.transform.rotation, ModeSelectCameraPoint.rotation, fraComplete);

        if (fraComplete > 0.3f)
        {
            camPanMode = false;
            menu = false;

        }
    }

    public void zoomToMapSelect()
    {
        fraComplete = fraComplete + 0.01f;

        Vector3 desiredPosition = MapSelectCamera.transform.position;
        Vector3 camPositionCurrent = camPosition.transform.position;

        menuCam.transform.position = Vector3.Slerp(camPositionCurrent, desiredPosition, fraComplete);

        menuCam.transform.rotation = Quaternion.Slerp(camPosition.transform.rotation, MapSelectCamera.rotation, fraComplete);

        if (fraComplete > 0.3f)
        {
            camPanMap = false;
            menu = false;

        }
    }

    public void resetCamera()
    {
        fraComplete = fraComplete + 0.01f;

        Vector3 desiredPosition = startingPosition.transform.position;
        Vector3 camPositionCurrent = camPosition.transform.position;

        menuCam.transform.position = Vector3.Slerp(camPositionCurrent, desiredPosition, fraComplete);
        menuCam.transform.rotation = Quaternion.Slerp(menuCam.transform.rotation, startingPosition.transform.rotation, fraComplete);

        if (fraComplete > 0.3f)
        {
            camPanModeBack = false;
            menu = true;

        }

    }

}
