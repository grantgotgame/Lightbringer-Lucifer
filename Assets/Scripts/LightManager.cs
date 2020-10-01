using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private float lightDampener = 10f;

    private Light lightSource;

    private PlayerController playerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        lightSource = GetComponent<Light>();
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        lightSource.intensity = playerControllerScript.stamina / lightDampener;
    }
}
