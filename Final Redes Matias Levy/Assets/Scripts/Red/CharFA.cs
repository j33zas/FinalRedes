using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharFA : MonoBehaviour
{
    //guns
    public GunFA[] Guns;
    GunFA currentGun;
    int currentGunIndex = 0;
    //stats
    public int maxHP;
    int _currHP;
    public float maxSpeed;
    float _currSpeed;
    public int acceleration;
    //unity
    Rigidbody2D _RB;
    Animator _AN;
    //misc
    int _score = 0;
    bool _isControllable;
    public bool Control
    {
        get
        {
            return _isControllable;
        }
        set
        {
            _isControllable = value;
        }
    }
    private void Awake()
    {
        _RB = GetComponent<Rigidbody2D>();
        _AN = GetComponent<Animator>();
        _currSpeed = maxSpeed;
        _currHP = maxHP;
        currentGun = Guns[currentGunIndex];
        _isControllable = true;
    }

    public void Move(Vector2 dir)
    {
        transform.position += new Vector3(dir.x, dir.y, transform.position.z) * maxSpeed * Time.deltaTime;
    }

    public void Look(Vector3 v3)
    {
        transform.up = v3;
    }

    public void Shoot()
    {
        currentGun.Shoot();
        _AN.SetTrigger("Shoot");
        _AN.SetInteger("GunID", currentGunIndex);
    }

    public void Reload()
    {

    }

    public IEnumerator Die()
    {
        _AN.SetBool("Dead", true);
        //pantalla ByN
        //color en gris
        _score -= 5;
        if (_score < 0)
            _score = 0;
        _isControllable = false;
        yield return new WaitForSeconds(3);
        _isControllable = true;
    }

    public void Respawn()
    {

    }

    public void ChangeWPN()
    {
        Guns[currentGunIndex].gameObject.SetActive(false);
        currentGunIndex++;
        if (currentGunIndex > Guns.Length - 1)
            currentGunIndex = 0;

        Guns[currentGunIndex].gameObject.SetActive(true);
        currentGun = Guns[currentGunIndex];
    }

    public void TakeDMG(int D)
    {
        _currHP -= D;
        if(_currHP<=0)
        {
            ServerCustom.server.RequestDie(this);
        }
    }

    public void SpawnIn(Vector3 position)
    {
        transform.position = position;
        _AN.SetBool("Dead", false);
    }
}
