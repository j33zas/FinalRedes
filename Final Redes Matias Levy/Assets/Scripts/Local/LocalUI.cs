using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LocalUI : MonoBehaviourPun
{
    public Text score = null;
    public Image health = null;
    public Image DMG = null;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void AddScore(int points)
    {
        score.text = "Score: " + points;
    }

    public void TakeDMG(float maxHP, float currHP)
    {
        health.fillAmount = currHP / maxHP;
        while (DMG.fillAmount - health.fillAmount > 0.01f)
            DMG.fillAmount = Mathf.Lerp(DMG.fillAmount, health.fillAmount, Time.deltaTime);
    }
}
