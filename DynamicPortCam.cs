using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicPortCam : MonoBehaviour
{
    public bool returnZoom;
    public bool mapZoom;
    public bool upgradeZoom;
    public bool boatZoom;
    public bool buyZoom;
    public bool swordZoom;

    private float fraComplete = 0.0f;

    [Header("Player Camera & Boat")]
    public GameObject playerCamera;


    [Header("Desired Camera Positions")]
    public Transform mapPos;
    public Transform upgradePos;
    public Transform boatPos;
    public Transform buyPos;
    public Transform sfPos;

    public Transform MainCameraPosition;

    private Vector3 currentCamPosition;
    private Transform currentCamTransform;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentCamPosition = playerCamera.transform.position;
        currentCamTransform = playerCamera.transform;

        //Vector3 mPos = mapPos.transform.position;
        //Vector3 uPos = upgradePos.transform.position;
        //Vector3 bPos = boatPos.transform.position;
        //Vector3 buPos = buyPos.transform.position;
        //Vector3 mCP = MainCameraPosition.transform.position;
         

        if (Input.GetKeyDown(KeyCode.M))
        {
            mapZoom = true;
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            returnZoom = true;
        }

        if (mapZoom)
        {
            dynamicCam(mapPos, mapPos.transform.position, ref mapZoom);
        }

        if (upgradeZoom)
        {
            dynamicCam(upgradePos, upgradePos.transform.position, ref upgradeZoom);
        }

        if (boatZoom)
        {
            dynamicCam(boatPos, boatPos.transform.position, ref boatZoom);
        }

        if (buyZoom)
        {
            dynamicCam(buyPos, buyPos.transform.position, ref buyZoom);
        }

        if (swordZoom)
        {
            dynamicCam(sfPos, sfPos.transform.position, ref swordZoom);
        }


        if (returnZoom)
        {
            dynamicCam(MainCameraPosition, MainCameraPosition.transform.position, ref returnZoom);
        }

    }

    public void dynamicCam(Transform desPosT, Vector3 desPos, ref bool zoomBool)
    {
        if (zoomBool == true)
        {

            fraComplete = fraComplete + 0.01f;

            playerCamera.transform.position = Vector3.Slerp(currentCamPosition, desPos, fraComplete); 

            playerCamera.transform.rotation = Quaternion.Slerp(currentCamTransform.transform.rotation, desPosT.transform.rotation, fraComplete);

            //playerCamera.transform.rotation = mapPos.transform.rotation;


            if (fraComplete > 0.3f)
            {
                fraComplete = 0.0f;
                zoomBool = false;
            }
        }

    }
}
