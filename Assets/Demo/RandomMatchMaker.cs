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
    //?C???X?y?N?^?[??????????????
    public GameObject PhotonObject;
    public GameObject PhotonObject2;
    [SerializeField] GameObject StartPosi;
    [SerializeField] GameObject StartPosi2;
    [SerializeField] GameObject joinRoomCanvas;
    [SerializeField] GameObject readyRoomCanvas;
    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject UICamera;

    [SerializeField] Button joinRoomBtn;
    [SerializeField] Button readyBtn;
    [SerializeField] TextMeshProUGUI pName1;
    [SerializeField] TextMeshProUGUI pName2;


    [SerializeField] TextMeshProUGUI playerName;

    private void Awake()
    {

        UICamera.SetActive(true);
        joinRoomCanvas.SetActive(true);
        readyRoomCanvas.SetActive(false);
        mainCanvas.SetActive(false);

        joinRoomBtn.onClick.AddListener(joinRoomClick);
       
    }


    void Start()
    {
       
    }

    // ?????f??
    void Update()
    {
        
    }

    void joinRoomClick()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NickName = playerName.text;
    }

    //?Z?c?]?N?X???g???o????
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    //?????????r?[?????p???????????A?????????????????????B
    public override void OnJoinedLobby()
    {
       
        PhotonNetwork.JoinRandomRoom();

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom("roomName", roomOptions);
    }

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
        hashtable["Message"] = "こんにちは";
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }


    
}