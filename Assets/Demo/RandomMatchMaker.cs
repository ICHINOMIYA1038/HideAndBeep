using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Cinemachine;

public class RandomMatchMaker : MonoBehaviourPunCallbacks
{
    //インスペクターから設定できる
    public GameObject PhotonObject;
    

    void Start()
    {
        //先ほどの設定を用いてサーバーに接続できる。
        PhotonNetwork.ConnectUsingSettings();
    }

    // このデモ
    void Update()
    {
        
    }

    //セツゾクスルトヨバレル
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    //今回はロビーを私用しないため、そのまま部屋に入れる。
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
        GameObject Player = PhotonNetwork.Instantiate(
                PhotonObject.name,
                new Vector3(0f, 1f, 0f),
                Quaternion.identity,
                0
                );
        Player.GetComponentInChildren<Camera>().enabled = true;
        Player.GetComponentInChildren<Camera>().depth -= 1;
        Player.GetComponentInChildren<CinemachineFreeLook>().enabled = true;


    }
    
}