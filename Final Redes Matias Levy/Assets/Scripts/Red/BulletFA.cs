using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class BulletFA : MonoBehaviourPun
{
    public int DMG;
    public float Speed;
    CharFA OW;
    public CharFA OwnerCharacter
    {
        get
        {
            return OW;
        }
        set
        {
            OW = value;
        }
    }

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
        if (C != OW && C)
            C.ReceiveDamage(DMG, OW);
        ServerCustom.server.DestroyMe(gameObject);
    }
}
