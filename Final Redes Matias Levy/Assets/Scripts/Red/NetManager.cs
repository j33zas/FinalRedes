using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetManager : MonoBehaviourPunCallbacks
{
    public GameObject[] buttons;
    public ServerCustom server;
    public LobbyManager lobby;

    string _playerName;
    string _serverID;

    public InputField nameIN;
    public InputField serverIN;
    public Text warnings;

    RoomOptions _RoomOPS = new RoomOptions();

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
        _RoomOPS.MaxPlayers = 5;//4 jugadores + server
        foreach (var item in buttons)
            item.SetActive(false);
    }

    private void Update()
    {
        _playerName = nameIN.text;
        _serverID = serverIN.text;
        PhotonNetwork.LocalPlayer.NickName = nameIN.text;
        if(!string.IsNullOrEmpty(_serverID) && !string.IsNullOrEmpty(_playerName))
            foreach (var item in buttons)
                if(!item.activeSelf)
                    item.SetActive(true);
    }

    public void ConnectToServer()
    {
        if(!PhotonNetwork.JoinRoom(_serverID))
        {
            warnings.text += "Lobby full" + "/n";
        }
    }

    public void CreateServer()
    {
        if(!PhotonNetwork.CreateRoom(_serverID, _RoomOPS, TypedLobby.Default))
        {
            warnings.text += "Lobby name taken" + "/n";
        }
    }

    public void JoinOrCreate()
    {
        if(PhotonNetwork.JoinOrCreateRoom(_serverID, _RoomOPS, TypedLobby.Default))
        {
            warnings.text += "can't join/create lobby" + "/n";
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
    }

    public override void OnCreatedRoom()
    {
        PhotonNetwork.Instantiate(server.name, Vector3.zero, Quaternion.identity);
        PhotonNetwork.Instantiate(lobby.name, Vector3.zero, Quaternion.identity);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {

    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {

    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {

    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
