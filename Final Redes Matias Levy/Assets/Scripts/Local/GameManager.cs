using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform[] respawnPos;
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
    }

    public Vector3 GetRandomPLSpawnPosition()
    {
        var pos = Random.Range(0, respawnPos.Length - 1);
        //verificar si la posicion tiene otro jugado cerca
        return respawnPos[pos].position;
    }
}
