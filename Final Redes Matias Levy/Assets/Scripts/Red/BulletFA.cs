using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class BulletFA : MonoBehaviourPun
{
    public int DMG;
    public float Speed;
    public CharFA OwnerCharacter;
    public Player OwnerPL;

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
        Player PL = coll.gameObject.GetComponent<Player>();
        if (C != OwnerCharacter && C && PL != null)
            ServerCustom.server.RequestPlayerDMG(PL, OwnerPL, DMG);
        ServerCustom.server.DestroyMe(gameObject);
    }
}
