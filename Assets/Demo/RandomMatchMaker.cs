using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Cinemachine;

public class RandomMatchMaker : MonoBehaviourPunCallbacks
{
    //�C���X�y�N�^�[����ݒ�ł���
    public GameObject PhotonObject;
    

    void Start()
    {
        //��قǂ̐ݒ��p���ăT�[�o�[�ɐڑ��ł���B
        PhotonNetwork.ConnectUsingSettings();
    }

    // ���̃f��
    void Update()
    {
        
    }

    //�Z�c�]�N�X���g���o����
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    //����̓��r�[�����p���Ȃ����߁A���̂܂ܕ����ɓ����B
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