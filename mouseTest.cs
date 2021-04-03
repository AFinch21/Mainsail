using UnityEngine;
using System.Collections;

public class mouseTest : MonoBehaviour
{
    GameObject test;

    Color col = Color.red;
    Color m_OriginalColor;
    MeshRenderer m_Renderer;

    void Start()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        m_OriginalColor = m_Renderer.material.color;

    }
    void OnMouseOver()
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        Debug.Log("Red");
        m_Renderer.material.color = Color.red;
    }
    void OnMouseExit()
    {
        m_Renderer.material.color = m_OriginalColor;
    }
}
