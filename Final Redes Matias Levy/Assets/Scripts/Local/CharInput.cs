using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharInput : MonoBehaviourPun
{
    float moveDirx;
    float moveDiry;
    Vector2 moveDir;
    Vector2 mousePos;
    CharFA _me;
    public CharFA Controller
    {
        get
        {
            return _me;
        }
        set
        {
            _me = value;
        }
    }
    Player PL;
    public Player mePL
    {
        get
        {
            return PL;
        }
        set
        {
            PL = value;
        }
    }
    private void Start()
    {
        mePL = PhotonNetwork.LocalPlayer;
        StartCoroutine(Tick());
    }
    
    IEnumerator Tick()
    {
        while(true)
        {
            yield return new WaitForSeconds(ServerCustom.TickRate);
            moveDirx = Input.GetAxis("Horizontal");
            moveDiry = Input.GetAxis("Vertical");
            moveDir = new Vector2(moveDirx, moveDiry);
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 lookDir = new Vector2(_me.transform.position.x, _me.transform.position.y) - mousePos;
            if (_me.HasControl)
            {
                ServerCustom.server.RequestMove(PhotonNetwork.LocalPlayer, moveDir);
                ServerCustom.server.RequestLook(PhotonNetwork.LocalPlayer, lookDir);
                if (Input.GetMouseButton(0))//m1 apretado
                    ServerCustom.server.RequestShoot(PhotonNetwork.LocalPlayer);
                if (Input.GetKeyDown(KeyCode.F))
                    ServerCustom.server.RequestDie(PhotonNetwork.LocalPlayer);
                if (Input.GetKeyDown(KeyCode.Q))
                    ServerCustom.server.RequestChangeWPN(PhotonNetwork.LocalPlayer);
                if (Input.GetKeyDown(KeyCode.R))
                    ServerCustom.server.RequestReload(PhotonNetwork.LocalPlayer);
            }
        }
    }
}
