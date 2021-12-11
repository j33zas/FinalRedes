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
    public LocalUI UIPF;
    LocalUI _characterUI;
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
        _characterUI = Instantiate(UIPF, Vector3.zero, Quaternion.identity);
        _characterUI.gameObject.SetActive(false);
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
    public void RequestSpawnPL(CharFA CH, Vector3 pos)
    {
        if (CharFAToPL.ContainsKey(CH))
            photonView.RPC("SpawnPlayerRPC", _serverPL, CharFAToPL[CH], pos);
        if(_serverPL != PhotonNetwork.LocalPlayer)
            _characterUI.gameObject.SetActive(true);
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
        if(PLToCharFA.ContainsKey(PL))
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
        if(PLToCharFA.ContainsKey(PL))
            PLToCharFA[PL].Shoot();
    }
    #endregion

    #region player Damage
    public void RequestPlayerDMG(CharFA CharHit, CharFA CharHitter, int DMG)
    {
        if (CharFAToPL.ContainsKey(CharHit) && CharFAToPL.ContainsKey(CharHitter))
            photonView.RPC("PlayerDMG", _serverPL, CharHit, CharFAToPL[CharHitter], DMG);
        if(CharFAToPL.ContainsKey(CharHit) && _serverPL != PhotonNetwork.LocalPlayer)
        {
            Player PL = CharFAToPL[CharHit];
            photonView.RPC("UIHealth", PL, CharHit.HP, CharHit.maxHP);
        }
    }

    [PunRPC]
    void PlayerDMG(CharFA DmgReceiver, Player hitter, int DMG)
    {
        DmgReceiver.TakeDMG(DMG, PLToCharFA[hitter]);
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
        if(photonView.IsMine)
        {
            PLToCharFA[PL].Die(GameManager.GM.diePenalty);
            //PLToCharFA[PLk].Score(GameManager.GM.killPoints);
        }
    }
    #endregion

    #region Player Change Weapon
    public void RequestChangeWPN(Player PL)
    {
        photonView.RPC("ChangeWPN", _serverPL, PL);
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
        photonView.RPC("ReloadRPC", _serverPL, PL);
    }
    [PunRPC]
    void ReloadRPC(Player PL)
    {
        PLToCharFA[PL].Reload();    
    }
    #endregion

    [PunRPC]
    void UIHealth(int curr, int max)
    {
        if(_characterUI)
            _characterUI.TakeDMG(curr, max);
    }
}
