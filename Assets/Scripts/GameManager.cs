using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : SingletonMonoBehaviour<GameManager>
{

    string[] playerName;
    int pNum = 0;

    bool isP1dead;
    bool isP2dead;

    PlayerController[] playerControllers;

    int score = 0;

    public int Score
    {
        set
        {

        }
        get
        {
            return score;
        }
    }

    public void Awake()
    {
        if (this != Instance)
        {
            Destroy(gameObject);
            return;
        }


        DontDestroyOnLoad(gameObject);

        playerName = new string[2];
        playerControllers = new PlayerController[2];



    }
    void Start()
    {

    }

    void Update()
    {

    }

    void addNewPlayer(PlayerController controller)
    {
        pNum += 1;

        playerControllers[pNum] = controller;
        playerName[pNum] = controller.getName();
    }
}