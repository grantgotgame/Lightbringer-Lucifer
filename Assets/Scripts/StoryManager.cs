using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    public string powerupText;

    private GameManager gameManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Change story text when collecting powerup
    private void OnTriggerEnter(Collider other)
    {
        if (gameManagerScript.gameHasStarted)
        {
            gameManagerScript.UpdateStoryText(powerupText);
        }
    }
}
