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
    //unity
    Rigidbody2D _RB;
    Animator _AN;
    CharInput _input;
    public CharInput inputPF;
    LocalUI _UI;
    public LocalUI MyUI
    {
        get
        {
            return _UI;
        }
        set
        {
            _UI = value;
        }
    }
    //misc
    int _score = 0;
    bool _isControllable;
    public bool HasControl
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
        DontDestroyOnLoad(gameObject);

        #region defino input
        _input = Instantiate(inputPF, Vector3.zero, Quaternion.identity);
        _input.Controller = this;
        _input.transform.parent = transform;
        #endregion

        #region GetComponents
        _RB = GetComponent<Rigidbody2D>();
        _AN = GetComponentInChildren<Animator>();
        #endregion

        #region incializo las armas
        foreach (var gun in Guns)
            gun.gameObject.SetActive(false);
        currentGun = Guns[currentGunIndex];
        currentGun.gameObject.SetActive(true);
        #endregion

        _currSpeed = maxSpeed;
        _currHP = maxHP;
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
        if(currentGun.Shoot())
        {
            _AN.SetInteger("GunIndex", currentGunIndex);
            _AN.SetTrigger("Shoot");
        }
    }

    public void Reload()
    {
        //_AN.SetTrigger("Reload");
        //_AN.SetInteger("GunIndex", currentGunIndex);
        StartCoroutine(currentGun.Reload());
    }

    public IEnumerator Die()
    {
        _AN.SetBool("Dead", true);
        _score -= 5;
        if (_score < 0)
            _score = 0;
        _isControllable = false;
        yield return new WaitForSeconds(5);
        ServerCustom.server.RequestSpawnPL(this, GameManager.GM.GetRandomPLSpawnPosition());
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
        //HUD.TakeDMG(_currHP, maxHP);
        if(_currHP<=0)
            ServerCustom.server.RequestDie(_input.mePL);
    }

    public void ReceiveDamage(int DMG)
    {
        ServerCustom.server.RequestPlayerDMG(_input.mePL, DMG);
    }

    public void SpawnIn(Vector3 position)
    {
        transform.position = position;
        _currHP = maxHP;
        foreach (var gun in Guns)
            gun.Respawn();
        _AN.SetBool("Dead", false);
    }
}
