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
    public int HP
    {
        get
        {
            return _currHP;
        }
    }
    public float maxSpeed;
    float _currSpeed;
    public bool dead;
    public float timeToRespawn;
    float _currTimeToRespawn;
    //unity
    Rigidbody2D _RB;
    Animator _AN;
    Collider2D _COLL;
    CharInput _input;
    public CharInput inputPF;
    CharFA lastDamager;
    
    //misc
    public int _score = 0;
    bool HasControl = true;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        #region defino input
        var temp = FindObjectsOfType<CharInput>();
        if(temp.Length < 1)
        {
            _input = Instantiate(inputPF, Vector3.zero, Quaternion.identity);
            _input.Controller = this;
            _input.transform.parent = transform;
        }
        #endregion

        #region GetComponents
        _RB = GetComponent<Rigidbody2D>();
        _AN = GetComponentInChildren<Animator>();
        _COLL = GetComponent<Collider2D>();
        #endregion

        #region incializo las armas
        foreach (var gun in Guns)
            gun.gameObject.SetActive(false);
        currentGun = Guns[currentGunIndex];
        currentGun.gameObject.SetActive(true);
        #endregion

        _currSpeed = maxSpeed;
        _currHP = maxHP;
        _currTimeToRespawn = timeToRespawn;
    }

    public void Move(Vector2 dir)
    {
        if(HasControl)
            transform.position += new Vector3(dir.x, dir.y, transform.position.z) * maxSpeed * Time.deltaTime;
    }

    public void Look(Vector3 v3)
    {
        if(HasControl)
            transform.up = v3;
    }

    public void Shoot()
    {
        if (!photonView.IsMine)
            return;
        if(HasControl)
        {
            if(currentGun.Shoot())
            {
                _AN.SetInteger("GunIndex", currentGunIndex);
                _AN.SetTrigger("Shoot");
            }
        }
    }

    public void Reload()
    {
        if(HasControl)
        {
            _AN.SetBool("Reloading", true);
            StartCoroutine(currentGun.Reload());
        }
    }

    public void EndReload()
    {
        _AN.SetBool("Reloading", false);
    }

    public void Die(int scoreLoss)
    {
        _AN.SetBool("Dead", true);
        HasControl = false;
        dead = true;
        _score -= scoreLoss;
        if (_score < 0)
            _score = 0;
        StartCoroutine(RespawnTimer());
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

    public void ReceiveDamage(int DMG, CharFA damager)
    {
        ServerCustom.server.RequestPlayerDMG(this, damager, DMG);
    }

    public void TakeDMG(int D, CharFA hitter)
    {
        _currHP -= D;
        lastDamager = hitter;
        if (_currHP<=0)
            ServerCustom.server.RequestDie(this, lastDamager);
    }

    #region RESPAWN
    IEnumerator RespawnTimer()
    {
        while (dead)
        {
            yield return new WaitForSeconds(ServerCustom.TickRate);
            HasControl = false;
            _currTimeToRespawn -= Time.deltaTime;
            if (_currTimeToRespawn <= 0)
                Respawn();
        }
    }

    public void Respawn()
    {
        ServerCustom.server.RequestSpawnPL(this, GameManager.GM.GetRandomPLSpawnPosition());
    }

    public void SpawnIn(Vector3 position)
    {
        transform.position = position;
        _currHP = maxHP;
        HasControl = true;
        dead = false;
        _currTimeToRespawn = timeToRespawn;
        foreach (var gun in Guns)
            gun.Respawn();
        _AN.SetBool("Dead", false);
    }
    #endregion

    public void Score(int pointsAdded)
    {
        _score += pointsAdded;
        if(_score > GameManager.GM.winningScore)
        {
            //ServerCustom.server.requestWin();
            Debug.LogError(name + " IS THE WINNER");
        }
    }
}
