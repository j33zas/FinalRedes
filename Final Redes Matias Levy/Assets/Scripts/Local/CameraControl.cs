using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    GameObject player;
    Vector3 point;
    Camera cam;
    public float influence;
    private void Start()
    {
        cam = GetComponent<Camera>();
        player = gameObject.transform.parent.gameObject;
    }

    private void Update()
    {
        point = (cam.ScreenToWorldPoint(Input.mousePosition) - player.transform.position) / influence;
        Vector3 player2D = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        transform.position = player2D + new Vector3(point.x, point.y, 0);
    }
}
