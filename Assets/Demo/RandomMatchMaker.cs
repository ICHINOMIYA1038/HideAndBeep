using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Cinemachine;

public class RandomMatchMaker : MonoBehaviourPunCallbacks
{
    //?C???X?y?N?^?[??????????????
    public GameObject PhotonObject;
    

    void Start()
    {
        //???????????????p?????T?[?o?[?????????????B
        PhotonNetwork.ConnectUsingSettings();
    }

    // ?????f??
    void Update()
    {
        
    }

    //?Z?c?]?N?X???g???o????
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    //?????????r?[?????p???????????A?????????????????????B
    public override void OnJoinedLobby()
    {
        PhotonNetwork.NickName = "youichi";
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
        GameObject player = PhotonNetwork.Instantiate(
                PhotonObject.name,
                new Vector3(0f, 1f, 0f),
                Quaternion.identity,
                0
                );
        GameManager gamemanager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        player.GetComponentInChildren<Camera>().enabled = true;
        player.GetComponentInChildren<Camera>().depth -= 1;
        player.GetComponentInChildren<CinemachineFreeLook>().enabled = true;
        PlayerController playerController = player.GetComponent<PlayerController>();

        playerController.setName("taro");
        gamemanager.addNewPlayer(playerController);


    }
    
}