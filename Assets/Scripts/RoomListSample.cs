using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

public class RoomListSample : MonoBehaviourPunCallbacks
{
    private RoomList roomList = new RoomList();

    public override void OnJoinedLobby()
    {
        roomList.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> changedRoomList)
    {
        roomList.Update(changedRoomList);
    }

    public override void OnLeftLobby()
    {
        roomList.Clear();
    }
}