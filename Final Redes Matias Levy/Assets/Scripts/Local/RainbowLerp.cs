using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainbowLerp : MonoBehaviour
{
    Color[] colors =
    {
        new Color(255,0,0,1),//red
        new Color(255,255,0,1),//yellow
        new Color(0,255,0,1),//green
        new Color(0,255,255,1),//light blue
        new Color(0,0,255,1),//blue
        new Color(255,0,255,1),//purple
    };
    public float timePerColor;
    float _currentTimePerColor;
    int currentC = 0;
    MaskableGraphic graphic;
    private void Awake()
    {
        _currentTimePerColor = timePerColor;
        graphic = GetComponent<MaskableGraphic>();
        graphic.color = colors[currentC];
    }
    void Update()
    {
        _currentTimePerColor -= Time.deltaTime;
        if(_currentTimePerColor <= 0)
        {
            if (currentC < colors.Length-1)
                currentC++;
            else
                currentC = 0;
            _currentTimePerColor = timePerColor;
            graphic.color = colors[currentC];
        }

    }
}
