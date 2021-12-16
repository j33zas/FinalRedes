using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class ServerCustom : MonoBehaviourPun
{
    //server stuff
    public static ServerCustom server;
    public CharFA charControllerPF;
    public EndScreen EndScreenPF;
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
            photonView.RPC("AddPlayer", _serverPL, PhotonNetwork.LocalPlayer);
    }
    [PunRPC]
    void AddPlayer(Player PL)
    {
        CharFA charInstance = PhotonNetwork.Instantiate(charControllerPF.name, Vector3.zero, Quaternion.identity).GetComponent<CharFA>();
        PLToCharFA.Add(PL, charInstance);
        CharFAToPL.Add(charInstance, PL);
        if (PLToCharFA.Count >= 2 && !allPLin)//cambiar a 4 para cumplir
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
        foreach (var PL_CHpair in PLToCharFA)
            PL_CHpair.Value.Initialize(PL_CHpair.Key);
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
        if (PhotonNetwork.IsMasterClient && GO != null)
            PhotonNetwork.Destroy(GO);
    }
    #endregion

    #region Player spawn    
    public void RequestSpawnPL(CharFA CH, Vector3 pos)
    {
        if (CharFAToPL.ContainsKey(CH))
            photonView.RPC("SpawnPlayerRPC", _serverPL, CharFAToPL[CH], pos);
    }
    [PunRPC]
    void SpawnPlayerRPC(Player PL, Vector3 pos)
    {
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

    #region player Damage
    public void RequestPlayerDMG(CharFA CharHit, CharFA CharHitter, int DMG)
    {
        if (CharFAToPL.ContainsKey(CharHit) && CharFAToPL.ContainsKey(CharHitter))
        {
            photonView.RPC("PlayerDMG", _serverPL, CharFAToPL[CharHit], CharFAToPL[CharHitter], DMG);
        }
    }

    [PunRPC]
    void PlayerDMG(Player DmgReceiver, Player hitter, int DMG)
    {
        if (PLToCharFA.ContainsKey(DmgReceiver) && PLToCharFA.ContainsKey(hitter))
            PLToCharFA[DmgReceiver].TakeDMG(DMG, PLToCharFA[hitter]);
    }
    #endregion

    #region Player Die
    public void RequestDie(CharFA CH, CharFA CHHitter)
    {
        if (CharFAToPL.ContainsKey(CH) && CharFAToPL.ContainsKey(CHHitter))
            photonView.RPC("PlayerDie", _serverPL, CharFAToPL[CH], CharFAToPL[CHHitter]);
    }

    [PunRPC]
    void PlayerDie(Player PL, Player PLk)
    {
        PLToCharFA[PL].Die(GameManager.GM.diePenalty);
        PLToCharFA[PLk].Score(GameManager.GM.killPoints);
    }
    #endregion

    #region Player Change Weapon
    //public void RequestChangeWPN(Player PL)
    //{
    //    photonView.RPC("ChangeWPN", _serverPL, PL);
    //}
    //[PunRPC]
    //void ChangeWPN(Player PL)
    //{
    //    if (PLToCharFA.ContainsKey(PL) && photonView.IsMine)
    ////        PLToCharFA[PL].ChangeWPN();
    ////}
    #endregion

    #region Player Reload Weapon
    public void RequestReload(Player PL)
    {
        photonView.RPC("ReloadRPC", _serverPL, PL);
    }
    [PunRPC]
    void ReloadRPC(Player PL)
    {
        PLToCharFA[PL].Reload();
    }
    #endregion

    #region Win!
    public void RequestWin(CharFA CH)
    {
        if (!PLToCharFA.ContainsValue(CH))
            return;
        List<string> names = new List<string>();
        var temp = PLToCharFA.OrderBy(pairs => pairs.Value.score);  //ordeno por score
        foreach (var item in PLToCharFA)
            names.Add(item.Key.NickName);                           //guardo en la lista provisoria
        PhotonNetwork.Instantiate(EndScreenPF.name, Vector3.zero, Quaternion.identity).GetComponent<EndScreen>();
        photonView.RPC("RPCWinner", RpcTarget.All, names.ToArray());
    }

    [PunRPC]
    void RPCWinner(string[]names)
    {
        var screen = FindObjectOfType<EndScreen>();
        screen.LoadNames(names);
        PhotonNetwork.LoadLevel("Lobby");
    }
    #endregion
}
