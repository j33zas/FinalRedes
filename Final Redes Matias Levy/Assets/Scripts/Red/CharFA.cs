using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharFA : MonoBehaviour
{
    public int rifleMaxAmmo;
    int _rifleCurrAmmo;
    public int pistolMaxAmmo;
    int _pistolCurrAmmo;
    public int maxHP;
    int _currHP;
    public float maxSpeed;
    float _currSpeed;
    public int acceleration;
    Rigidbody2D _RB;
    int _score = 0;
    private void Awake()
    {
        _RB = GetComponent<Rigidbody2D>();
        _rifleCurrAmmo = rifleMaxAmmo;
        _pistolCurrAmmo = pistolMaxAmmo;
        //_currSpeed = 0;
        _currSpeed = maxSpeed;
        _currHP = maxHP;
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

    public void Parry()
    {

    }

    public void TakeDMG()
    {

    }

    public void SpawnIn()
    {
        transform.position = GameManager.GM.GetRandomPLSpawnPosition();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //ServerCustom.server.RequestLoseHP(dmg);
    }
}
