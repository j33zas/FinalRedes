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
    public float reloadTime;

    public Transform shootPos;
    public BulletFA bullet;

    private void Start()
    {
        StartCoroutine(Tick());
        _currAmmo = maxAmmo;
    }

    IEnumerator Tick()
    {
        while(true)
        {
            yield return new WaitForSeconds(1 / 60);
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

    public void Shoot()
    {
        if(_canShoot && _currAmmo > 0 && PhotonNetwork.IsMasterClient)
        {
            Vector3 recoilV = new Vector3(0, 0, Random.Range(-recoil, recoil));
            BulletFA B = PhotonNetwork.Instantiate(bullet.name, shootPos.position, shootPos.rotation).GetComponent<BulletFA>();
            B.transform.Rotate(recoilV);
            _canShoot = false;
            _currAmmo--;
        }
    }

    public IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        _currAmmo = maxAmmo;
    }
}
