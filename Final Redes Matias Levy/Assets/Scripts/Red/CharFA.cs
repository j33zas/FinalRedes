using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharFA : MonoBehaviourPun
{
    //guns
    public GunFA Gun;
    //stats
    public int maxHP;
    [SerializeField]
    int _HP;
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
    public LocalUI UIPF;
    LocalUI UI;
    Player MEPL;
    Camera MyCamera;
    public Camera CamPF;
    //misc
    public int score = 0;
    bool HasControl = true;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        #region GetComponents
        _RB = GetComponent<Rigidbody2D>();
        _AN = GetComponentInChildren<Animator>();
        _COLL = GetComponent<Collider2D>();
        #endregion

        Gun.Respawn();
        _currSpeed = maxSpeed;
        _HP = maxHP;
        _currTimeToRespawn = timeToRespawn;
    }

    public void Move(Vector2 dir)
    {
        if(HasControl)
            transform.position += new Vector3(dir.x, dir.y, transform.position.z) * _currSpeed * Time.deltaTime;
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
            if(Gun.Shoot())
                _AN.SetTrigger("Shoot");
    }

    public void Reload()
    {
        if(HasControl)
        {
            _AN.SetBool("Reloading", true);
            StartCoroutine(Gun.Reload());
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
        score -= scoreLoss;
        if (score < 0)
            score = 0;
        photonView.RPC("ScoreUI", MEPL, score);
        StartCoroutine(RespawnTimer());
    }

    public void ReceiveDamage(int DMG, CharFA damager)
    {
        ServerCustom.server.RequestPlayerDMG(this, damager, DMG);
    }

    public void TakeDMG(int D, CharFA hitter)
    {
        _HP -= D;
        photonView.RPC("HealthUI", MEPL, _HP, maxHP);
        lastDamager = hitter;
        if (_HP<=0)
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
        _HP = maxHP;
        HasControl = true;
        dead = false;
        _currTimeToRespawn = timeToRespawn;
        Gun.Respawn();
        photonView.RPC("ResetHP", MEPL);
        _AN.SetBool("Dead", false);
    }
    #endregion

    public void Score(int pointsAdded)
    {
        score += pointsAdded;
        photonView.RPC("ScoreUI", MEPL, score);
        if(score > GameManager.GM.winningScore)
        {
            ServerCustom.server.RequestWin(this);
        }
    }

    public void Initialize(Player target)
    {
        photonView.RPC("RPCInitialize", target);
        MEPL = target;
    }
    #region UI local RPCs
    [PunRPC]
    void RPCInitialize()
    {
        #region defino input
        _input = Instantiate(inputPF, Vector3.zero, Quaternion.identity);
        _input.Controller = this;
        _input.transform.parent = transform;
        #endregion

        UI = Instantiate(UIPF, Vector3.zero, Quaternion.identity);
        MyCamera = Instantiate(CamPF, new Vector3(transform.position.x, transform.position.y, -10), Quaternion.identity);
        MyCamera.transform.parent = transform;
        _input.cam = MyCamera;
        _HP = maxHP;
    }

    [PunRPC]
    void HealthUI(int curr, int max)
    {
        UI.TakeDMG(max, curr);
    }

    [PunRPC]
    void ScoreUI(int score)
    {
        UI.AddScore(score);
    }
    [PunRPC]
    void ResetHP()
    {
        UI.ResetHPValues();
    }
    [PunRPC]
    void CameraMoveRPC()
    {

    }
    #endregion
}
