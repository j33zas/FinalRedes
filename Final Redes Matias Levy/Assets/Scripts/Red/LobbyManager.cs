using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPun
{
    public static LobbyManager lobby;
    public Button PlayButton;
    public Text youAretxt;
    public Text playercount;
    public Text serverName;
    public Text[] PlayerNames;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        lobby = this;
        PlayButton.gameObject.SetActive(false);
        if (ServerCustom.server.GetServerPlayer() == PhotonNetwork.LocalPlayer)
            youAretxt.text += "The Server";
        else
            youAretxt.text += PhotonNetwork.LocalPlayer.NickName;
    }
    private void Update()
    {
        var PLs = PhotonNetwork.PlayerList;
        playercount.text = PhotonNetwork.PlayerList.Length - 1 + "/4 Players";
        for (int i = 1; i < PLs.Length; i++)
        {
            if (PLs[i] != ServerCustom.server.GetServerPlayer())
                PlayerNames[i-1].text = PhotonNetwork.PlayerList[i].NickName;
        }
        serverName.text = "Lobby name: " + PhotonNetwork.CurrentRoom.Name;
    }
    public void AllPlayersIn()
    {
        if(PhotonNetwork.LocalPlayer == ServerCustom.server.GetServerPlayer())//solo el server puede poner play
            PlayButton.gameObject.SetActive(true);
        PlayButton.onClick.AddListener(ServerCustom.server.StartLevel);
    }
}
