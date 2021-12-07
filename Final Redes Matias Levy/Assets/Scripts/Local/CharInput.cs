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
    private void Start()
    {
        StartCoroutine(Tick());
    }
    
    IEnumerator Tick()
    {
        while(_me.HasControl)
        {
            yield return new WaitForSeconds(ServerCustom.TickRate);
            moveDirx = Input.GetAxis("Horizontal");
            moveDiry = Input.GetAxis("Vertical");
            moveDir = new Vector2(moveDirx, moveDiry);
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 lookDir = new Vector2(_me.transform.position.x, _me.transform.position.y) - mousePos;

            ServerCustom.server.RequestMove(_me, moveDir);
            ServerCustom.server.RequestLook(_me, lookDir);

            if (Input.GetMouseButton(0))//m1 apretado
                ServerCustom.server.RequestShoot(_me);

            if (Input.GetKeyDown(KeyCode.R))
                ServerCustom.server.RequestReload(_me);
        }
    }
}
