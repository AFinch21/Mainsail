using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MapLoader : MonoBehaviour
{
    public void mapLoader()
    {
        if(GameObject.Find("Iona").activeSelf == true)
        {
            //SceneManager.LoadScene("Iona_Map_v1");
            Debug.Log("MAP");
        }
    }
}
