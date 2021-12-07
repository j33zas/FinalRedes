using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GunFA : MonoBehaviourPun
{
    public float fireRate;
    float _timeTillNextShot;
    public int maxAmmo;
    int _currAmmo;
    public float recoil;
    bool _canShoot = true;
    bool _reloading = false;
    public float reloadTime;

    public Transform shootPos;
    public BulletFA bullet;
    public CharFA owner;

    private void Start()
    {
        StartCoroutine(Tick());
        _currAmmo = maxAmmo;
    }

    IEnumerator Tick()
    {
        while(true)
        {
            yield return new WaitForSeconds(ServerCustom.TickRate);
            if (!_canShoot)
            {
                _timeTillNextShot += Time.deltaTime;
                if(_timeTillNextShot >= fireRate)
                {
                    _canShoot = true;
                    _timeTillNextShot = 0;
                }
            }
        }
    }

    public bool Shoot()
    {
        if(_canShoot && _currAmmo > 0 && !_reloading && PhotonNetwork.IsMasterClient)
        {
            Vector3 recoilV = new Vector3(0, 0, Random.Range(-recoil, recoil));
            BulletFA B = PhotonNetwork.Instantiate(bullet.name, shootPos.position, shootPos.rotation).GetComponent<BulletFA>();
            B.transform.Rotate(recoilV);
            B.Owner = owner;
            _canShoot = false;
            _currAmmo--;
            return true;
        }
        return false;
    }
    public void Respawn()
    {
        _currAmmo = maxAmmo;
        _timeTillNextShot = 0;
        _canShoot = true;
    }
    public IEnumerator Reload()
    {
        _reloading = true;
        yield return new WaitForSeconds(reloadTime);
        owner.EndReload();
        _reloading = false;
        _currAmmo = maxAmmo;
    }
}
