using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviour
{
    public Transform[] respawnPos;
    CharFA[] controllers;
    public float cherckerArea;
    public static GameManager GM
    {
        get
        {
            return _GM;
        }
    }
    static GameManager _GM;
    private void Awake()
    {
        _GM = this;
        controllers = ServerCustom.server.GetAllControllers();
        if (PhotonNetwork.IsMasterClient)
            foreach (var PL in controllers)
                ServerCustom.server.RequestSpawnPL(PL, GetRandomPLSpawnPosition());
    }

    public Vector3 GetRandomPLSpawnPosition()
    {
        var pos = Random.Range(0, respawnPos.Length - 1);
        //verificar si la posicion tiene otro jugado cerca
        //si tiene, pedir otra
        return respawnPos[pos].position;
    }
}
