using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonScript : MonoBehaviour
{

    public GameObject definedButton;
    public UnityEvent OnClick = new UnityEvent();
    Color col = Color.red;
    Color m_OriginalColor;
    MeshRenderer m_Renderer;
    bool onButton;
    public LayerMask Highlighter;
    public LayerMask def;
    private bool poo;



    // Use this for initialization
    void Start()
    {
        definedButton = this.gameObject;

        m_Renderer = GetComponent<MeshRenderer>();
        m_OriginalColor = m_Renderer.material.color;

    }

    // Update is called once per frame
    void Update()
    {
        onButton = false;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit Hit;
        poo = false;

        if (Physics.Raycast(ray, out Hit, Highlighter))
        {
            Debug.Log("Red");
            m_Renderer.material.color = Color.red;
            poo = true;

        }
        if (!Physics.Raycast(ray, out Hit) && !poo)
        {
            m_Renderer.material.color = m_OriginalColor;
            Debug.Log("Green");
        }

    }


}
