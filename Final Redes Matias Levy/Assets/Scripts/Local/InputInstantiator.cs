using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class InputInstantiator : MonoBehaviourPun
{
    public CharInput inputPF;

    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
            Instantiate(inputPF, Vector3.zero, Quaternion.identity);
        Destroy(gameObject);
    }
}
