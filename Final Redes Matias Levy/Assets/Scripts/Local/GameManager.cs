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
    public int winningScore = 300;
    static GameManager _GM;
    public static GameManager GM
    {
        get
        {
            return _GM;
        }
    }
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
        return respawnPos[pos].position;
    }
}
