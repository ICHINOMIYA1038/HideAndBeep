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
    int pNum = 0;
    public int sceneState = 0;

    bool isP1dead;
    bool isP2dead;

    [SerializeField] GameObject mainItemPanel;
    [SerializeField] GameObject subItemPanel1;
    [SerializeField] GameObject subItemPanel2;
    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] GameObject gameClearCanvas;
    [SerializeField] GameObject InputAssist;
    [SerializeField] TextMeshProUGUI player1Name;
    [SerializeField] TextMeshProUGUI player2Name;
    [SerializeField] ZombieController[] zombieCon;
    [SerializeField] Image playerImg1;
    [SerializeField] Image playerImg2;
    [SerializeField] Sprite img0;
    [SerializeField] Sprite img1;
    [SerializeField] Sprite img2;
    [SerializeField] GameObject gameClearCamera;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip gameClearSE;
    [SerializeField] AudioClip gameOverSE;
    [SerializeField] TextMeshProUGUI messageBox;
    Queue<string> stringList = new Queue<string>();
    int maxTextSize = 5;

    [SerializeField] PlayerController playerController;



    int score = 0;

    public void gameOver(){

        mainCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);
        StartCoroutine("gameOverPanel");
        Debug.Log("gameover");

    }

    public void gameClear()
    {
        mainCanvas.SetActive(false);
        gameClearCanvas.SetActive(true);
        StartCoroutine("gameClearPanel");
        playerController.gameClear();
        gameClearCamera.SetActive(true);
        audioSource.PlayOneShot(gameClearSE);


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

    IEnumerator gameClearPanel()
    {
        CanvasGroup canvasGroup = gameClearCanvas.GetComponent<CanvasGroup>();
        yield return new WaitForSeconds(2.5f);
        for (int i = 0; i < 500; i++)
        {
            canvasGroup.alpha += 0.002f;
            yield return null;

        }
        float alpha=0;
        for (int i = 0; i < 500; i++)
        {
            alpha = i*0.002f;
            gameClearCanvas.GetComponentInChildren<Image>().color = new Vector4(0f, 0f, 0f, alpha);
            yield return null;

        }

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

    }
    void Start()
    {
        playerImg1.sprite = img0;
        playerImg2.sprite = img0;
    }

    void AddText(string text)
    {
        Debug.Log(text);
        stringList.Enqueue(text);
        if (stringList.Count > maxTextSize)
        {
            stringList.Dequeue();
        }

        messageBox.text = "";
        foreach(var elem in stringList)
        {
            messageBox.text += $"{elem}\n";
        }


    }

    void Update()
    {

    }

    public void enemyDamaged(Vector3 position,float range)
    {
        foreach (ZombieController zombie in zombieCon)
        {
           if((zombie.transform.position - position).magnitude < range)
            {
                zombie.damaged();
            }
        }
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
       

        playerController = controller;
        player1Name.SetText(PhotonNetwork.PlayerList[0].NickName);
        playerImg1.sprite = img1;
        var players = PhotonNetwork.PlayerList;
        if (players.Length == 1)
        {
            player1Name.SetText(players[0].NickName);
        }
        if (players.Length == 2)
        {
            player1Name.SetText(players[0].NickName);
            player2Name.SetText(players[1].NickName);
        }

    }

    
    

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        
        AddText($"{otherPlayer.NickName}が退出しました。");
        if (!otherPlayer.IsLocal)
        {
            StartCoroutine("DisconnectMaster");
        }
        player2Name.SetText("NoPlayer");
        playerImg2.sprite = img0;
    }

    IEnumerator DisconnectMaster()
    {
        AddText("マスターが退出したため、自動で終了します。");
        AddText("5");
        yield return new WaitForSeconds(1);
        AddText("4");
        yield return new WaitForSeconds(1);
        AddText("3");
        yield return new WaitForSeconds(1);
        AddText("2");
        yield return new WaitForSeconds(1);
        AddText("1");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        PhotonNetwork.Disconnect();

    }

    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        AddText($"{otherPlayer.NickName}が入室しました。");
        var players = PhotonNetwork.PlayerList;
        player1Name.SetText(players[0].NickName);
        player2Name.SetText(players[1].NickName);
        playerImg2.sprite = img2;

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        
        // カスタムプロパティが更新されたプレイヤーのプレイヤー名とIDをコンソールに出力する
        //Debug.Log($"{targetPlayer.NickName}({targetPlayer.ActorNumber})");

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