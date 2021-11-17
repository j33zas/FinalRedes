using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFA : MonoBehaviour
{
    public float DMG;
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

    public BulletFA(float D, float S, float L)
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
            yield return new WaitForSeconds(1 / 60);
            transform.position += transform.up * Speed * Time.deltaTime;
        }
    }
    private void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log("wall");
        if(coll.gameObject.GetComponent<CharFA>() != owner)
            ServerCustom.server.DestroyMe(gameObject);
    }
}
