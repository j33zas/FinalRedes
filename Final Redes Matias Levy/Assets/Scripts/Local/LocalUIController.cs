using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalUIController : MonoBehaviour
{
    public LocalUI UI;
    int hp = 100;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            hp -= 10;
            UI.TakeDMG(100, hp);
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            hp = 100;
            UI.TakeDMG(100, hp);
        }
    }
}
