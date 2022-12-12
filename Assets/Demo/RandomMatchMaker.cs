using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Cinemachine;
using UnityEngine.UI;
using TMPro;

public class RandomMatchMaker : MonoBehaviourPunCallbacks
{
    //プレイヤーのプレハブ1
    public GameObject PhotonObject;
    //プレイヤーのプレハブ2
    public GameObject PhotonObject2;
    //プレイヤー1のスポーン位置
    [SerializeField] GameObject StartPosi;
    //プレイヤー2のスポーン位置
    [SerializeField] GameObject StartPosi2;
    //名前を入力し、ロビーに入室するときのキャンバス
    [SerializeField] GameObject joinRoomCanvas;
    //ロビーに入室後、準備完了するためのキャンバス
    [SerializeField] GameObject readyRoomCanvas;
    //メイン画面のキャンバス
    [SerializeField] GameObject mainCanvas;
    /// <summary>
    /// UIを表示させるためのカメラ
    /// </summary>
    [SerializeField] GameObject UICamera;
    ///ロビーに入室するときのボタン
    [SerializeField] Button joinRoomBtn;
    ///準備完了するときのボタン
    [SerializeField] Button readyBtn;
    ///プレイヤー1の名前
    [SerializeField] TextMeshProUGUI pName1;
    ///プレイヤー2の名前
    [SerializeField] TextMeshProUGUI pName2;
    ///入力を受付するときのテキストボックス内のテキスト
    [SerializeField] TextMeshProUGUI playerName;

    private void Awake()
    {

        UICamera.SetActive(true);
        joinRoomCanvas.SetActive(true);
        readyRoomCanvas.SetActive(false);
        mainCanvas.SetActive(false);
        joinRoomBtn.onClick.AddListener(joinRoomClick);
       
    }

    /// <summary>
    /// PhotonNetworkに接続
    /// ニックネームを決定
    /// </summary>
    void joinRoomClick()
    {   
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NickName = playerName.text;
    }

    /// <summary>
    /// 接続できたらランダムな部屋に入室
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    
    public override void OnJoinedLobby()
    {
       
        PhotonNetwork.JoinRandomRoom();

    }

    /// <summary>
    /// 接続できなかったときの処理
    /// 接続できないときに部屋を作成する。
    /// 最大人数は2人
    /// </summary>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom("roomName", roomOptions);
    }

    /// <summary>
    /// ロビーに接続完了したときの処理。
    /// 名前などを確定して、準備完了ボタンを表示する
    /// </summary>
    public override void OnJoinedRoom()
    {
        var players = PhotonNetwork.PlayerList;
        if (players.Length == 1)
        {
            pName1.text = players[0].NickName;
        }
        else if (players.Length == 2)
        {
            pName1.text = players[0].NickName;
            pName2.text = players[1].NickName;
        }
        else
        {
            Debug.Log("Error");

        }
        joinRoomCanvas.SetActive(false);
        readyRoomCanvas.SetActive(true);
        mainCanvas.SetActive(false);

        readyBtn.onClick.AddListener(onReady);

    }

    /// <summary>
    /// ReadyButtonを押した時の処理
    /// プレイヤーインスタンスを生成し、メインキャンバスをアクティブにする。
    /// 他のキャンバスはoff
    /// </summary>
    public void onReady()
    {
        
        UICamera.SetActive(false);
        joinRoomCanvas.SetActive(false);
        readyRoomCanvas.SetActive(false);
        mainCanvas.SetActive(true);
        GameObject player;
        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 spawnPosition = StartPosi.transform.position;
             player = PhotonNetwork.Instantiate(
                    PhotonObject.name,
                    spawnPosition,
                    Quaternion.identity,
                    0
                    );
        }
        else
        {
            Vector3 spawnPosition2 = StartPosi2.transform.position;
             player = PhotonNetwork.Instantiate(
                    PhotonObject2.name,
                    spawnPosition2,
                    Quaternion.identity,
                    0
                    );
        }
        GameManager gamemanager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        gamemanager.sceneState = 1;
        player.GetComponentInChildren<Camera>().enabled = true;
        player.GetComponentInChildren<Camera>().depth -= 1;
        player.GetComponentInChildren<CinemachineFreeLook>().enabled = true;
        PlayerController playerController = player.GetComponent<PlayerController>();

        playerController.setName(PhotonNetwork.NickName);
        gamemanager.addNewPlayer(playerController,playerController.getPhotonView());

        var hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable["ItemState"] = 0;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }


    
}