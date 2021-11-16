using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class ServerCustom : MonoBehaviourPun
{
    //server stuff
    public static ServerCustom server;
    public CharFA charControllerPF;
    public CharInput charInputPF;
    Player _server;
    Dictionary<Player, CharFA> PlayerControllers = new Dictionary<Player, CharFA>();
    bool allPLin;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (!server)
            if (photonView.IsMine)
                photonView.RPC("SetServer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        allPLin = false;
    }

    [PunRPC]
    void SetServer(Player PL)
    {
        if (server)
        {
            Destroy(gameObject);
            return;
        }
        server = this;
        _server = PL;
        if (PhotonNetwork.LocalPlayer != _server)
        {
            photonView.RPC("AddPlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        }
    }

    [PunRPC]
    void AddPlayer(Player PL)
    {
        CharFA charInstance = PhotonNetwork.Instantiate(charControllerPF.name, Vector3.zero, Quaternion.identity).GetComponent<CharFA>();
        PlayerControllers.Add(PL, charInstance);
        DontDestroyOnLoad(charInstance);
        if (PlayerControllers.Count >= 1 && !allPLin)//cambiar a 4 para cumplir
        {
            if (LobbyManager.lobby != null)
                LobbyManager.lobby.AllPlayersIn();
            allPLin = true;
        }
    }

    #region inicio nivel
    public void StartLevel()
    {
        photonView.RPC("LoadLevel", RpcTarget.AllBuffered);
    }
    [PunRPC]
    void LoadLevel()
    {
        PhotonNetwork.LoadLevel("PlayScene");
        if (photonView.IsMine)
            PhotonNetwork.Destroy(LobbyManager.lobby.gameObject);
    }
    #endregion

    #region Misc Functions
    public Player GetServerPlayer()
    {
        if (_server != null)
            return _server;
        else
            return null;
    }
    public CharFA GetControllerFromPL(Player PL)
    {
        if (PlayerControllers.ContainsKey(PL))
            return PlayerControllers[PL];
        else
            return null;
    }
    #endregion

    #region mover posicion
    public void RequestMove(Player PL, Vector2 dir)
    {
        photonView.RPC("MovePL", RpcTarget.All, PL, dir);
    }
    [PunRPC]
    void MovePL(Player PL, Vector2 dir)
    {
        if (PlayerControllers.ContainsKey(PL))
            PlayerControllers[PL].Move(dir);
    }
    #endregion

    #region mirar al mouse
    public void RequestLook(Player PL, Vector3 v3)
    {
        photonView.RPC("LookPL", RpcTarget.All, PL, v3);
    }
    [PunRPC]
    void LookPL(Player PL, Vector3 v3)
    {
        if (PlayerControllers.ContainsKey(PL))
            PlayerControllers[PL].Look(v3);
    }
    #endregion

}
