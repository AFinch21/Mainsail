using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlighterScript : MonoBehaviour
{
    Color col = Color.red;
    Color m_OriginalColor;
    MeshRenderer m_Renderer;
    DynamicPortCam dpm;
    Outline outline;

    public GameObject highlightText;
    public Transform CamPos;

    public GameObject Camera;

    void Awake()
    {
        outline = gameObject.GetComponent<Outline>();
        Camera = GameObject.Find("BoatSeletionCamera");
        outline.enabled = false;
        highlightText.SetActive(false);
    }

    void Start()
    {
        m_Renderer = GetComponent<MeshRenderer>();

        m_OriginalColor = m_Renderer.material.color;

    }

    private void Update()
    {


    }
    void OnMouseOver()
    {
        //Debug.Log(gameObject.name);

        if (gameObject.name == "MapTable")
        {
            outline.enabled = true;
            highlightText.SetActive(true);
            if (Input.GetMouseButtonDown(0))
            {
                Camera.GetComponent<DynamicPortCamera>().desiredPosition = CamPos;
                Camera.GetComponent<DynamicPortCamera>().zoom = true;

            }
        }

        if (gameObject.name == "Crane")
        {
            outline.enabled = true;
            highlightText.SetActive(true);
            if (Input.GetMouseButtonDown(0))
            {
                Camera.GetComponent<DynamicPortCamera>().desiredPosition = CamPos;
                Camera.GetComponent<DynamicPortCamera>().zoom = true;
            }
        }

        if (gameObject.name == "BoatSelectionTableTop")
        {
            outline.enabled = true;
            highlightText.SetActive(true);
            if (Input.GetMouseButtonDown(0))
            {
                Camera.GetComponent<DynamicPortCamera>().desiredPosition = CamPos;
                Camera.GetComponent<DynamicPortCamera>().zoom = true;
            }
        }

        if (gameObject.name == "Noteboard")
        {
            outline.enabled = true;
            highlightText.SetActive(true);
            if (Input.GetMouseButtonDown(0))
            {
                Camera.GetComponent<DynamicPortCamera>().desiredPosition = CamPos;
                Camera.GetComponent<DynamicPortCamera>().zoom = true;
            }
        }


    }
    void OnMouseExit()
    {
        //m_Renderer.material.color = m_OriginalColor;

        if (gameObject.name == "MapTable")
        {
            outline.enabled = false;
            highlightText.SetActive(false);
        }

        if (gameObject.name == "Crane")
        {
            outline.enabled = false;
            highlightText.SetActive(false);
        }

        if (gameObject.name == "BoatSelectionTableTop")
        {
            outline.enabled = false;
            highlightText.SetActive(false);

        }

        if (gameObject.name == "Noteboard")
        {
            outline.enabled = false;
            highlightText.SetActive(false);

        }

    }

}
