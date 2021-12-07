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
    [SerializeField]
    int _currHP;
    public float maxSpeed;
    float _currSpeed;
    bool dead;
    public float timeToRespawn;
    float _currTimeToRespawn;
    //unity
    Rigidbody2D _RB;
    Animator _AN;
    CharInput _input;
    public CharInput inputPF;
    public Player PL;
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

        _currSpeed = maxSpeed;
        _currHP = maxHP;
        _isControllable = true;
        _currTimeToRespawn = timeToRespawn;
    }
    private void Start()
    {
        #region incializo las armas
        foreach (var gun in Guns)
        {
            gun.gameObject.SetActive(false);
        }
        currentGun = Guns[currentGunIndex];
        currentGun.gameObject.SetActive(true);
        #endregion
        
    }
    IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitForSeconds(ServerCustom.TickRate);
            if (dead)
            {
                HasControl = false;
                _currTimeToRespawn -= Time.deltaTime;
                if (_currTimeToRespawn <= 0)
                    Respawn();
            }
        }   
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

    public void Die()
    {
        _AN.SetBool("Dead", true);
        HasControl = false;
        dead = true;
        _score -= 20;
        if (_score < 0)
            _score = 0;
    }

    public void Respawn()
    {
        ServerCustom.server.RequestSpawnPL(this, GameManager.GM.GetRandomPLSpawnPosition());
        _isControllable = true;
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

    public void TakeDMG(int D, CharFA hitter)
    {
        _currHP -= D;
        lastDamager = hitter;
        Debug.LogError(name + " took damage from " + hitter.name);
        if (_currHP<=0)
            ServerCustom.server.RequestDie(this, lastDamager);
    }

    public void ReceiveDamage(int DMG, CharFA damager)
    {
        ServerCustom.server.RequestPlayerDMG(this, damager, DMG);
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
            //ServerCustom.server.requestWin();
        }
    }
}
