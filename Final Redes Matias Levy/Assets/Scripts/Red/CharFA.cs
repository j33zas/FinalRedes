using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharFA : MonoBehaviour
{
    //guns
    public GunFA Rifle;
    public GunFA pistol;
    GunFA currentGun;
    //stats
    public int maxHP;
    int _currHP;
    public float maxSpeed;
    float _currSpeed;
    public int acceleration;
    //unity
    Rigidbody2D _RB;
    //misc
    int _score = 0;
    private void Awake()
    {
        _RB = GetComponent<Rigidbody2D>();
        _currSpeed = maxSpeed;
        _currHP = maxHP;
        currentGun = Rifle;
    }

    public void Move(Vector2 dir)
    {
        if (dir != Vector2.zero)
        {
            //if (_currSpeed < maxSpeed)
            //    _currSpeed += Time.deltaTime * acceleration;
            //else
            //    _currSpeed = maxSpeed;
            _RB.AddForce(dir * maxSpeed * Time.deltaTime, ForceMode2D.Impulse);
        }
        else
        {
            _RB.velocity = Vector2.zero;
            //_currSpeed = 0;
        }

        if (_RB.velocity.magnitude >= maxSpeed)
            _RB.velocity = dir * maxSpeed;
    }

    public void Look(Vector3 v3)
    {
        transform.up = v3;
    }

    public void Shoot()
    {
        currentGun.Shoot();
    }

    public void Reload()
    {

    }

    public void Die()
    {

    }

    public void Respawn()
    {

    }

    public void ChangeWPN()
    {

    }

    public void TakeDMG()
    {

    }

    public void SpawnIn(Vector3 position)
    {
        transform.position = position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //ServerCustom.server.RequestLoseHP(dmg);
    }
}
