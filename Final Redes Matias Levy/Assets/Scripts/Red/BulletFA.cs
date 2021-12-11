using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class BulletFA : MonoBehaviourPun
{
    public int DMG;
    public float Speed;
    public CharFA Owner;

    private void Start()
    {
        StartCoroutine(Tick());
    }

    IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitForSeconds(ServerCustom.TickRate);
            transform.position += transform.up * Speed * Time.deltaTime;
        }
    }
    private void OnTriggerEnter2D(Collider2D coll)
    {
        CharFA C = coll.gameObject.GetComponent<CharFA>();
        if (C)
        {
            if(C != Owner && !C.dead)
            {
                ServerCustom.server.RequestPlayerDMG(C, Owner, DMG);
                ServerCustom.server.DestroyMe(gameObject);
            }
        }
        else
            ServerCustom.server.DestroyMe(gameObject);
    }
}
