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
    private void Start()
    {
        //if (!photonView.IsMine)
        //    return;
        _me = ServerCustom.server.GetControllerFromPL(PhotonNetwork.LocalPlayer);
        StartCoroutine(Tick());
    }
    void Update()
    {
        //moveDirx = Input.GetAxis("Horizontal");
        //moveDiry = Input.GetAxis("Vertical");
        //moveDir = new Vector2(moveDirx, moveDiry);
        //mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector3 lookDir = new Vector2(_me.transform.position.x, _me.transform.position.y) - mousePos;
        //if (hascontrol)
        //{
        //    ServerCustom.server.RequestMove(PhotonNetwork.LocalPlayer, moveDir);
        //    ServerCustom.server.RequestLook(PhotonNetwork.LocalPlayer, lookDir);
        //}
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

            if (_me.Control)
            {
                ServerCustom.server.RequestMove(PhotonNetwork.LocalPlayer, moveDir);
                ServerCustom.server.RequestLook(PhotonNetwork.LocalPlayer, lookDir);
                if (Input.GetMouseButton(0))//m1 apretado
                    ServerCustom.server.RequestShoot(PhotonNetwork.LocalPlayer);
                if (Input.GetKeyDown(KeyCode.F))
                    ServerCustom.server.RequestDie(_me);
                //if (Input.GetKeyDown(KeyCode.Q))
                    //ServerCustom.server.RequestChangeWPN(PhotonNetwork.LocalPlayer);
            }
        }
    }
}
