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
    Player _serverPL;
    Dictionary<Player, CharFA> PLToCharFA = new Dictionary<Player, CharFA>();
    Dictionary<CharFA, Player> CharFAToPL = new Dictionary<CharFA, Player>();
    bool allPLin;
    public Text debugTxt;
    static int TR = 256;
    public static int TickRate
    {
        get
        {
            return 1 / TR;
        }
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        allPLin = false;
        if (server == null && photonView.IsMine)
            photonView.RPC("SetServer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    void SetServer(Player PL)
    {
        server = this;
        _serverPL = PL;
        if (PhotonNetwork.LocalPlayer != _serverPL)
        {
            photonView.RPC("AddPlayer", _serverPL, PhotonNetwork.LocalPlayer);
        }
    }
    [PunRPC]
    void AddPlayer(Player PL)
    {
        CharFA charInstance = PhotonNetwork.Instantiate(charControllerPF.name, Vector3.zero, Quaternion.identity).GetComponent<CharFA>();
        PLToCharFA.Add(PL, charInstance);
        CharFAToPL.Add(charInstance, PL);
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
        if (photonView.IsMine)
            PhotonNetwork.Destroy(LobbyManager.lobby.gameObject);
        PhotonNetwork.LoadLevel("PlayScene");
    }
    #endregion

    #region Misc Functions
    public Player GetServerPlayer()
    {
        if (_serverPL != null)
            return _serverPL;
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

    public void DestroyMe(GameObject GO)
    {
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(GO);
    }
    #endregion

    #region Player spawn    
    public void RequestSpawnPL(CharFA FA, Vector3 pos)
    {
        Player PL = CharFAToPL[FA];
        photonView.RPC("SpawnPlayerRPC", _serverPL, PL, pos);
    }
    [PunRPC]
    void SpawnPlayerRPC(Player PL, Vector3 pos)
    {
        if (PLToCharFA.ContainsKey(PL))
            PLToCharFA[PL].SpawnIn(pos);
    }
    #endregion

    #region Player move
    public void RequestMove(Player PL, Vector2 dir)
    {
        photonView.RPC("MovePL", _serverPL, PL, dir);
    }
    [PunRPC]
    void MovePL(Player PL, Vector2 dir)
    {
        if (PLToCharFA.ContainsKey(PL))
            PLToCharFA[PL].Move(dir);
    }
    #endregion

    #region Player look
    public void RequestLook(Player PL, Vector3 v3)
    {
        photonView.RPC("LookPL", _serverPL, PL, v3);
    }
    [PunRPC]
    void LookPL(Player PL, Vector3 v3)
    {
        if (PLToCharFA.ContainsKey(PL))
            PLToCharFA[PL].Look(v3);
    }
    #endregion

    #region Player shoot
    public void RequestShoot(Player PL)
    {
        photonView.RPC("ShootPL", _serverPL, PL);
    }
    [PunRPC]
    void ShootPL(Player PL)
    {
        if (PLToCharFA.ContainsKey(PL))
            PLToCharFA[PL].Shoot();
    }
    #endregion

    #region player Hit
    public void RequestPlayerDMG(Player PL, int DMG)
    {
        photonView.RPC("PlayerDMG", RpcTarget.All, PL, DMG);
    }

    [PunRPC]
    void PlayerDMG(Player PL, int DMG)
    {
        if (PLToCharFA.ContainsKey(PL))
            PLToCharFA[PL].TakeDMG(DMG);
    }
    #endregion

    #region Player Die
    public void RequestDie(Player PL)
    {
        photonView.RPC("PlayerDie", RpcTarget.All, PL);
    }
    [PunRPC]
    void PlayerDie(Player PL)
    {
        if (PLToCharFA.ContainsKey(PL))
            StartCoroutine(PLToCharFA[PL].Die());
    }
    #endregion

    #region Player Change Weapon
    public void RequestChangeWPN(Player PL)
    {
        photonView.RPC("ChangeWPN", RpcTarget.All, PL);
    }
    [PunRPC]
    void ChangeWPN(Player PL)
    {
        if (PLToCharFA.ContainsKey(PL))
            PLToCharFA[PL].ChangeWPN();
    }
    #endregion

    #region Player Reload Weapon
    public void RequestReload(Player PL)
    {
        photonView.RPC("ReloadRPC", RpcTarget.All, PL);
    }
    [PunRPC]
    void ReloadRPC(Player PL)
    {
        if (PLToCharFA.ContainsKey(PL))
            PLToCharFA[PL].Reload();    
    }
    #endregion
}
