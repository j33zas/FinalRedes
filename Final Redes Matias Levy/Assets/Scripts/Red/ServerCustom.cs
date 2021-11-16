using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    Dictionary<Player, CharFA> PLToCharFA = new Dictionary<Player, CharFA>();
    Dictionary<CharFA, Player> CharFAToPL = new Dictionary<CharFA, Player>();
    bool allPLin;
    public Text debugTxt;
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
        PLToCharFA.Add(PL, charInstance);
        CharFAToPL.Add(charInstance, PL);
        DontDestroyOnLoad(charInstance);
        if (PLToCharFA.Count >= 1 && !allPLin)//cambiar a 4 para cumplir
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

    #region Spawn player    
    public void RequestSpawnPL(CharFA FA, Vector3 pos)
    {
        debugTxt.text = pos + "";
        Player PL;
        if (CharFAToPL.ContainsKey(FA))
        {
            PL = CharFAToPL[FA];
            photonView.RPC("SpawnPlayerRPC", RpcTarget.All, PL, pos);
        }
    }
    [PunRPC]
    void SpawnPlayerRPC(Player PL, Vector3 pos)
    {
        if (PLToCharFA.ContainsKey(PL))
            PLToCharFA[PL].SpawnIn(pos);
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
        if (PLToCharFA.ContainsKey(PL))
            return PLToCharFA[PL];
        else
            return null;
    }
    public CharFA[] GetAllControllers()
    {
        List<CharFA> result = new List<CharFA>();
        foreach (var PL in PLToCharFA)
            result.Add(PL.Value);
        return result.ToArray();
    }
    public Player[] GetAllPlayers()
    {
        List<Player> result = new List<Player>();
        foreach (var PL in PLToCharFA)
            result.Add(PL.Key);
        return result.ToArray();
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
        if (PLToCharFA.ContainsKey(PL))
            PLToCharFA[PL].Move(dir);
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
        if (PLToCharFA.ContainsKey(PL))
            PLToCharFA[PL].Look(v3);
    }
    #endregion

}
