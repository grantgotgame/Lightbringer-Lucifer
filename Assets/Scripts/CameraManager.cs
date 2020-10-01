using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private float positionOffset = .4f;
    private float lookOffset = 1f;

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void LateUpdate()
    {
        // Rise and fall with player
        transform.position = new Vector3(0, player.transform.position.y + positionOffset * player.transform.position.y, 0);

        // Look at player
        transform.LookAt(new Vector3(player.transform.position.x,
            player.transform.position.y + lookOffset, player.transform.position.z));
    }
}
