using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class GameManager : SingletonMonoBehaviour<GameManager>
{

    string[] playerName;
    int pNum = 0;
    public int sceneState = 0;

    bool isP1dead;
    bool isP2dead;

    [SerializeField] GameObject mainItemPanel;
    [SerializeField] GameObject subItemPanel1;
    [SerializeField] GameObject subItemPanel2;
    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] GameObject InputAssist;
    [SerializeField] TextMeshProUGUI player1Name;
    [SerializeField] TextMeshProUGUI player2Name;

    [SerializeField] Image playerImg1;
    [SerializeField] Image playerImg2;
    [SerializeField] Sprite img0;
    [SerializeField] Sprite img1;
    [SerializeField] Sprite img2;

    PlayerController[] playerControllers;



    int score = 0;

    public void gameOver(){

        mainCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);
        StartCoroutine("gameOverPanel");
        Debug.Log("gameover");

    }

    IEnumerator gameOverPanel()
    {
        CanvasGroup canvasGroup = gameOverCanvas.GetComponent<CanvasGroup>();
        yield return new WaitForSeconds(2);
        for (int i = 0; i < 200; i++)
        {
            canvasGroup.alpha += 0.005f;
            yield return null;

        }
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        PhotonNetwork.Disconnect();
    }

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
        sceneState　= 0;


        //DontDestroyOnLoad(gameObject);

        playerName = new string[2];
        playerControllers = new PlayerController[2];



    }
    void Start()
    {

    }

    void Update()
    {

    }

    public void ActiveInputAssist()
    {
        InputAssist.SetActive(true);
    }

    public void DeactiveInputAssist()
    {
        InputAssist.SetActive(false);
    }

    public void addNewPlayer(PlayerController controller)
    {
        pNum += 1;

        playerControllers[pNum] = controller;
        playerName[pNum] = controller.getName();
        player1Name.SetText(PhotonNetwork.PlayerList[0].NickName);
        playerImg1.sprite = img1;

    }
    

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName}left Room");
    }

    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        Debug.Log("enterRoom");
        var players = PhotonNetwork.PlayerList;
        player1Name.SetText(players[0].NickName);
        player2Name.SetText(players[1].NickName);
        playerImg2.sprite = img1;

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        
        // カスタムプロパティが更新されたプレイヤーのプレイヤー名とIDをコンソールに出力する
        Debug.Log($"{targetPlayer.NickName}({targetPlayer.ActorNumber})");

        // 更新されたプレイヤーのカスタムプロパティのペアをコンソールに出力する
        foreach (var prop in changedProps)
        {
            
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

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        // 更新されたルームのカスタムプロパティのペアをコンソールに出力する
        foreach (var prop in propertiesThatChanged)
        {
            Debug.Log($"{prop.Key}: {prop.Value}");
        }
    }

}