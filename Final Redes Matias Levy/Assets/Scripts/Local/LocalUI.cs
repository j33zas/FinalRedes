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
        StartCoroutine(FancyHP());
    }

    public void ResetHPValues()
    {
        health.fillAmount = 1;
        DMG.fillAmount = 1;
    }

    IEnumerator FancyHP()
    {
        while (DMG.fillAmount - health.fillAmount > 0.001f)
        {
            yield return new WaitForSeconds(ServerCustom.TickRate);
            DMG.fillAmount = Mathf.Lerp(DMG.fillAmount, health.fillAmount, Time.deltaTime);
        }
    }
}