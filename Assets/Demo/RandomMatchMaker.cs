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
    [SerializeField] GameObject StartPosi;
    [SerializeField] GameObject joinRoomCanvas;
    [SerializeField] GameObject readyRoomCanvas;
    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject UICamera;

    [SerializeField] Button joinRoomBtn;
    [SerializeField] Button readyBtn;

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
        roomOptions.MaxPlayers = 8;
        PhotonNetwork.CreateRoom("roomName", roomOptions);
    }

    public override void OnJoinedRoom()
    {
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
        Vector3 spawnPosition = StartPosi.transform.position;
        GameObject player = PhotonNetwork.Instantiate(
                PhotonObject.name,
                spawnPosition,
                Quaternion.identity,
                0
                );
        GameManager gamemanager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        player.GetComponentInChildren<Camera>().enabled = true;
        player.GetComponentInChildren<Camera>().depth -= 1;
        player.GetComponentInChildren<CinemachineFreeLook>().enabled = true;
        PlayerController playerController = player.GetComponent<PlayerController>();

        playerController.setName(PhotonNetwork.NickName);
        gamemanager.addNewPlayer(playerController);

        var hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable["ItemState"] = 0;
        hashtable["Message"] = "こんにちは";
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }


    
}