using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharFA : MonoBehaviourPun
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
    CharFA lastDamager;
    //misc
    public int _score = 0;
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
        {
            gun.gameObject.SetActive(false);
            gun.owner = this;
        }
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
        _AN.SetBool("Reloading", true);
        StartCoroutine(currentGun.Reload());
    }
    public void EndReload()
    {
        _AN.SetBool("Reloading", false);
    }

    public IEnumerator Die()
    {
        _AN.SetBool("Dead", true);
        HasControl = false;
        _score -= 20;
        if (_score < 0)
            _score = 0;
        yield return new WaitForSeconds(5);
        ServerCustom.server.RequestSpawnPL(this, GameManager.GM.GetRandomPLSpawnPosition());
        HasControl = true;
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
            ServerCustom.server.RequestDie(PhotonNetwork.LocalPlayer, lastDamager);
    }

    public void ReceiveDamage(int DMG, CharFA damager)
    {
        ServerCustom.server.RequestPlayerDMG(PhotonNetwork.LocalPlayer, DMG);
        lastDamager = damager;
    }

    public void SpawnIn(Vector3 position)
    {
        transform.position = position;
        _currHP = maxHP;
        foreach (var gun in Guns)
            gun.Respawn();
        _AN.SetBool("Dead", false);
    }

    public void Score(int pointsAdded)
    {
        _score += pointsAdded;
        if(_score > GameManager.GM.winningScore)
        {
            //ServerCustom.server.requestWin()
            Debug.LogError(PhotonNetwork.LocalPlayer.NickName + " is the winner!");
        }
    }
}
