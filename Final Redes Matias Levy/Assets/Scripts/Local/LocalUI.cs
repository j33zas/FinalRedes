using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalUI : MonoBehaviour
{
    [SerializeField]
    Text score = null;
    [SerializeField]
    Image health = null;
    [SerializeField]
    Image dmgTaken = null;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void AddScore(int points)
    {
        score.text = "Score: " + points;
    }

    public void TakeDMG(int currHP, int maxHP)
    {
        health.fillAmount = currHP / maxHP;
        StartCoroutine(CoolDMG(health.fillAmount));
    }
    IEnumerator CoolDMG(float filltarget)
    {
        while(dmgTaken.fillAmount - filltarget >= 0.01f)
        {
            dmgTaken.fillAmount = Mathf.Lerp(dmgTaken.fillAmount, filltarget, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
