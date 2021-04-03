using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject ProgressManager;


    public void backToMenu(bool save)
    {
        if (save == true)
        {
            SceneManager.LoadScene("MainMenu");
            ProgressManager.GetComponent<PlayerProgress>().SaveProgress();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }

    }
}
