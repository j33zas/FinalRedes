using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class EndScreen : MonoBehaviourPun
{
    public Text[] positions;
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void Menu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Menu");
        Destroy(gameObject);
        Destroy(ServerCustom.server);
    }

    public void Quit()
    {
        PhotonNetwork.LeaveRoom();
        Application.Quit();
    }

    public void LoadNames(string[] values)
    {
        for (int i = 0; i < values.Length; i++)
            if(values[i] != null)
                positions[i].text = values[i];
    }
}
