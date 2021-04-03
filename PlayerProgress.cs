using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerProgress : MonoBehaviour
{
    //Be sure you set your List public so that it can be accessed by other classes.
    public static List<string> boats = new List<string>();

    public bool orca;
    public bool swordfish;
    public bool porpoise;
    public int level = 1;
    public int cash = 5000;

    public TextMeshProUGUI cashText;
    public TextMeshProUGUI levelText;


    void Start()
    {

        LoadProgress();

        if (orca)
        {
            boats.Add("Orca");
        }
        if (swordfish)
        {
            boats.Add("Swordfish");
        }
        if (porpoise)
        {
            boats.Add("Porpoise");
        }
    }

    void Update()
    {
        cashText.text = cash.ToString();
        levelText.text = level.ToString();

        if (Input.GetKeyDown(KeyCode.W))
        {
            cash = cash + 100000;

        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            cash = cash - 100000;

        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            level = level + 1;

        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            level = level - 1;

        }

        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    buyBoat("Swordfish", 2000);
        //}

        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    sellBoat("Swordfish", 2000);
        //}

        foreach (var x in boats) {
            Debug.Log(x.ToString());
        }
    }

    public void SaveProgress()
    {
        SaveGame.SavePlayer(this);
    }

    public void LoadProgress()
    {
        PlayerData data = SaveGame.LoadPlayer();
        level = data.level;
        cash = data.cash;
        orca = data.orca;
        swordfish = data.swordfish;
        porpoise = data.porpoise;
        Debug.Log("Game Loaded - Level: " + data.level + ", cash: " + data.cash +
            ", boats you own: Swordfish " + data.swordfish +
            ", Orca: " + data.orca +
            ", Porpoise: " + data.porpoise);
    }

    public void buyBoat(string boatName, int boatValue)
    {
        if (!boats.Contains(boatName))
        {
            //swordfish = true;
            boats.Add(boatName);
            cash = cash - boatValue;
            Debug.Log(boatName + " bought for " + boatValue);
        }
        else if (boats.Contains(boatName))
        {
            Debug.Log("You already have this boat!");
        }
    }

    public void sellBoat(string boatName, int boatValue)
    {
        if (boats.Contains(boatName))
        {
            //swordfish = false;
            boats.Remove(boatName);
            int depreciation = (int)((float)boatValue * 0.5f);

            cash = cash + depreciation;
            Debug.Log(boatName + " bought for " + boatValue + " but sold with a depreciation 0.5 for " + depreciation);
            if (GameObject.Find(boatName).activeSelf)
            {
                GameObject.Find(boatName).SetActive(false);
            }
        }
    }


}
