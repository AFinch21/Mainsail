using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Winch : MonoBehaviour
{
    public GameObject winch;
    AudioSource audioData;


    // Update is called once per frame
    private void Start()
    {
        audioData = GetComponent<AudioSource>();
    }

    void Update()
    {
        audioData.loop = false;

        if (Input.GetKey(KeyCode.E))
        {
            winch.transform.Rotate(0.0f, 0.0f, 10.0f, Space.Self);
            audioData.loop = true;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            winch.transform.Rotate(0.0f, 0.0f, -10.0f, Space.Self);
            audioData.loop = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            audioData.Play();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            audioData.Play();
        }
    }
    public void playAudio()
    {
    }

}
