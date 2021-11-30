using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFA : MonoBehaviour
{
    public int DMG;
    public float Speed;
    public float Life;
    CharFA OW;
    public CharFA owner
    {
        set
        {
            OW = value;
        }
        get
        {
            return OW;
        }
    }

    public BulletFA(int D, float S, float L)
    {
        DMG = D;
        Speed = S;
        Life = L;
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
        if (C != owner && C != null)
            C.ReceiveDamage(DMG);
        ServerCustom.server.DestroyMe(gameObject);
    }
}
