using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatSelector : MonoBehaviour
{
    public int selectedBoat = 0;
    private PlayerProgress progressScript;

    public List<string> boatsInTheScene = new List<string>();

    private void Start()
    {
        boatsInTheScene = PlayerProgress.boats;
        boatsInTheScene.Add("No Boat");
    }

    // Start is called before the first frame update
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            boatRight();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            boatLeft();
        }


    }

    public void boatRight()
    {
        int previousSelectedBoat = selectedBoat;

        if (selectedBoat >= transform.childCount - 1)
            selectedBoat = 0;
        else
            selectedBoat++;

        if (previousSelectedBoat != selectedBoat)
        {
            SelectBoat();
        }


    }

    public void boatLeft()
    {
        int previousSelectedBoat = selectedBoat;

        if (selectedBoat <= 0)
            selectedBoat = transform.childCount - 1;
        else
            selectedBoat--;

        if (previousSelectedBoat != selectedBoat)
        {
            SelectBoat();
        }


    }

    void SelectBoat()
    {
        int i = 0;

        foreach (Transform boat in transform)
        {
            if (boatsInTheScene.Contains(boat.name))
            {
                if (i == selectedBoat)
                {
                    Debug.Log(boat.name);
                    boat.gameObject.SetActive(true);
                }
                else
                {
                    boat.gameObject.SetActive(false);
                }
                i++;
            }

        }
    }

}
