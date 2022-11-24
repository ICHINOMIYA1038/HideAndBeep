using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameManager : SingletonMonoBehaviour<GameManager>
{

    string[] playerName;
    int pNum = 0;

    bool isP1dead;
    bool isP2dead;

    [SerializeField] GameObject mainItemPanel;
    [SerializeField] GameObject subItemPanel1;
    [SerializeField] GameObject subItemPanel2;

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

    public void addNewPlayer(PlayerController controller)
    {
        pNum += 1;

        playerControllers[pNum] = controller;
        playerName[pNum] = controller.getName();
       
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // カスタムプロパティが更新されたプレイヤーのプレイヤー名とIDをコンソールに出力する
        Debug.Log($"{targetPlayer.NickName}({targetPlayer.ActorNumber})");

        // 更新されたプレイヤーのカスタムプロパティのペアをコンソールに出力する
        foreach (var prop in changedProps)
        {
            Debug.Log($"{prop.Key}: {prop.Value}");


            
            if (prop.Key.ToString() == "ItemState")
            {
                if(targetPlayer.IsLocal==true)
                {
                    foreach (Transform child in mainItemPanel.transform)
                    {
                        child.gameObject.SetActive(false);
                    }
                    mainItemPanel.transform.GetChild((int)prop.Value).gameObject.SetActive(true);

                    foreach (Transform child in subItemPanel1.transform)
                    {
                        child.gameObject.SetActive(false);
                    }
                    subItemPanel1.transform.GetChild((int)prop.Value).gameObject.SetActive(true);
                }
                if(targetPlayer.IsLocal==false)
                {

                    foreach (Transform child in subItemPanel2.transform)
                    {
                        child.gameObject.SetActive(false);
                    }
                    subItemPanel2.transform.GetChild((int)prop.Value).gameObject.SetActive(true);
                }
               
            }
                
        }

       


    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // 更新されたルームのカスタムプロパティのペアをコンソールに出力する
        foreach (var prop in propertiesThatChanged)
        {
            Debug.Log($"{prop.Key}: {prop.Value}");
        }
    }

}