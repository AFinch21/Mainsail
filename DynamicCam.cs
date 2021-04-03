using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCam : MonoBehaviour
{
    public bool miniMapZoom;
    public bool returnZoom;
    public bool zoomed;

    private float fraComplete = 0.0f;

    [Header("Player Camera & Boat")]
    public GameObject playerCamera;
    public Transform camTarget;


    [Header("Minimap Camera & UI")]
    public GameObject miniMapCam;
    public GameObject UI;
    public GameObject HelmUI;
    public GameObject seaMiniMap;
    public float camHeight = 2000f;


    [Header("Desired Camera Positions")]
    public Transform miniMapPos;
    public Transform helmCamPos;

    private Vector3 currentCamPosition;


    // Start is called before the first frame update
    void Start()
    {
        zoomed = false;
    }

    // Update is called once per frame
    void Update()
    {
        currentCamPosition = playerCamera.transform.position;
        Vector3 mmPos = miniMapPos.transform.position;
        Vector3 hcPos = helmCamPos.transform.position;


        if (Input.GetKeyDown(KeyCode.M))
        {
            seaMiniMap.SetActive(true);
            if (!zoomed)
            {
                seaMiniMap.SetActive(true);
                fraComplete = 0.0f;
                miniMapZoom = true;
                zoomed = true;
            } else if (zoomed)
            {
                seaMiniMap.SetActive(false);
                fraComplete = 0.0f;
                returnZoom = true;
                zoomed = false;
            }
        }

        //miniMapCam.transform.rotation.Set(90.0f, 0.0f, 0.0f, 0.0f);

        miniMapCam.transform.position = new Vector3(camTarget.position.x, camHeight, camTarget.position.z);

        if (Input.GetKeyDown(KeyCode.O))
        {
            camHeight = camHeight - 500f;
            Debug.Log("Zoom In");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            camHeight = camHeight + 500f;
            Debug.Log("Zoom In");
        }


        if (miniMapZoom)
        {
            dynamicCam(mmPos, ref miniMapZoom);
            miniMapCam.SetActive(true);
            UI.SetActive(true);
            HelmUI.SetActive(false);
        }

        if (returnZoom)
        {
            dynamicCam(hcPos, ref returnZoom);

            miniMapCam.SetActive(false);
            UI.SetActive(false);
            HelmUI.SetActive(true);

        }

    }

    public void dynamicCam(Vector3 desPos, ref bool zoomBool)
    {
        if(zoomBool == true)
        {
            fraComplete = fraComplete + 0.01f;

            playerCamera.transform.position = Vector3.Slerp(currentCamPosition, desPos, fraComplete);

            //playerCamera.transform.rotation = Quaternion.Slerp(currentCamPosition.transform.rotation, miniMapPos.rotation, fraComplete);

            if (fraComplete > 0.3f)
            {
                fraComplete = 0.0f;
                zoomBool = false;
            }
        }

    }
}
